using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using MassTransit;
using Application.Consumers;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPostInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PostDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPostDbContext>(provider => provider.GetRequiredService<PostDbContext>());

        services.AddMassTransit(x =>
        {
            x.AddConsumer<LikeOnPostConsumer>();

            x.AddEntityFrameworkOutbox<PostDbContext>(f =>
            {
                f.UsePostgres();
                f.UseBusOutbox();
            });

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"] ?? "rabbitmq", "/", h =>
                {
                    h.Username(configuration["RabbitMQ:Username"] ?? "guest");
                    h.Password(configuration["RabbitMQ:Password"] ?? "guest");
                });

                cfg.UseMessageRetry(r => r.Exponential(
                    4,
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(30),
                    TimeSpan.FromSeconds(3)
                ));

                cfg.ConfigureEndpoints(context);
            });
        });

        var minioSettings = configuration.GetSection("Minio");
        var endpoint = minioSettings["Endpoint"]!;
        var accessKey = minioSettings["AccessKey"];
        var secretKey = minioSettings["SecretKey"];
        var bucketName = minioSettings["BucketName"]!;

        services.AddSingleton<IMinioClient>(m =>
            new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .Build());

        services.AddScoped<IImageStorageService>(sp =>
            new ImageStorageService(
                sp.GetRequiredService<IMinioClient>(),
                bucketName,
                endpoint));

        return services;
    }
}