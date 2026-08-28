using Application.Consumers;
using Application.Interfaces;
using Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddLikeInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LikeDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ILikeDbContext>(provider => provider.GetRequiredService<LikeDbContext>());

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ContentNotFoundConsumer>();

            x.AddEntityFrameworkOutbox<LikeDbContext>(f =>
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

        return services;
    }
}