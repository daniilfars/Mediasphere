using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Shared.Observability;

public static class ObservabilityExtensions
{
    public static WebApplication UseTraceIdHeader(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            var activity = Activity.Current;
            if (activity is not null)
            {
                context.Response.Headers["X-Trace-Id"] = activity.TraceId.ToString();
            }

            await next();
        });

        return app;
    }

    public static WebApplication UseExternalCorrelationScope(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            var externalCorrelationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(externalCorrelationId))
            {
                await next();
                return;
            }

            externalCorrelationId = externalCorrelationId.Length > 128
                ? externalCorrelationId[..128]
                : externalCorrelationId;

            var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("ExternalCorrelation");
            using (logger.BeginScope(new Dictionary<string, object>
            {
                ["ExternalCorrelationId"] = externalCorrelationId
            }))
            {
                await next();
            }
        });

        return app;
    }

    public static WebApplicationBuilder AddObservability(this WebApplicationBuilder builder, string serviceName)
    {
        var configuration = builder.Configuration;
        var seqUrl = configuration["Seq:ServerUrl"];
        var seqApiKey = configuration["Seq:ApiKey"];
        var bufferFile = configuration["Seq:BufferBaseFilename"];

        builder.Host.UseSerilog((context, _, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Service", serviceName)
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                .Enrich.With<ActivityEnricher>()
                .WriteTo.Console(new RenderedCompactJsonFormatter());

            if (!string.IsNullOrWhiteSpace(seqUrl))
            {
                loggerConfiguration.WriteTo.Seq(
                    serverUrl: seqUrl,
                    apiKey: seqApiKey,
                    bufferBaseFilename: bufferFile);
            }
        });

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.Filter = context =>
                            !context.Request.Path.StartsWithSegments("/health") &&
                            !context.Request.Path.StartsWithSegments("/swagger");
                        options.RecordException = true;
                    })
                    .AddHttpClientInstrumentation(options => options.RecordException = true)
                    .AddGrpcClientInstrumentation()
                    .AddProcessor(new SensitiveDataActivityProcessor())
                    .AddSource("MassTransit")
                    .SetSampler(new ParentBasedSampler(
                        new TraceIdRatioBasedSampler(GetSamplingRatio(configuration))));

                var otlpEndpoint = configuration["OpenTelemetry:Otlp:Endpoint"];
                if (Uri.TryCreate(otlpEndpoint, UriKind.Absolute, out var endpoint))
                {
                    tracing.AddOtlpExporter(options =>
                    {
                        options.Endpoint = endpoint;
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;

                        if (!string.IsNullOrWhiteSpace(seqApiKey))
                        {
                            options.Headers = $"X-Seq-ApiKey={seqApiKey}";
                        }
                    });
                }
            });

        return builder;
    }

    private static double GetSamplingRatio(IConfiguration configuration)
    {
        var configuredValue = configuration["OpenTelemetry:SamplingRatio"];
        return double.TryParse(configuredValue, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out var ratio)
            ? Math.Clamp(ratio, 0, 1)
            : 1.0;
    }
}

internal sealed class SensitiveDataActivityProcessor : BaseProcessor<Activity>
{
    public override void OnEnd(Activity data)
    {
        data.SetTag("url.query", null);
        data.SetTag("url.full", null);
        data.SetTag("http.request.body", null);
        data.SetTag("http.response.body", null);
        data.SetTag("http.request.header.authorization", null);
        data.SetTag("http.request.header.cookie", null);
        data.SetTag("http.response.header.set_cookie", null);
    }
}

internal sealed class ActivityEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var activity = Activity.Current;
        if (activity is null)
        {
            return;
        }

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", activity.TraceId.ToString()));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SpanId", activity.SpanId.ToString()));
    }
}
