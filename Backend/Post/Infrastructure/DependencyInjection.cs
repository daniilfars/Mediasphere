using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPostInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PostDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPostDbContext, PostDbContext>();

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