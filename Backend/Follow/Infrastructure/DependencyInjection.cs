using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFollowInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FollowDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IFollowDbContext>(provider => provider.GetRequiredService<FollowDbContext>());

        return services;
    }
}