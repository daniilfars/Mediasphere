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
    public static IServiceCollection AddCommentInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CommentDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ICommentDbContext>(provider => provider.GetRequiredService<CommentDbContext>());

        services.AddMassTransit(x =>
        {
            x.AddConsumer<LikeOnCommentConsumer>();
            x.AddConsumer<LikeOnCommentDeletedConsumer>();

            x.AddEntityFrameworkOutbox<CommentDbContext>(f =>
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