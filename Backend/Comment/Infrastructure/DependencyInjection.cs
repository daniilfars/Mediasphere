using Application.Interfaces;
using Infrastructure.Data;
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

        return services;
    }
}