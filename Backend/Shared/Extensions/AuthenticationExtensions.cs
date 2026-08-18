using Microsoft.Extensions.DependencyInjection;

namespace Shared.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAppSecurity(this IServiceCollection services)
    {
        services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.Authority = "http://keycloak_server:8080/realms/mediasphere";
                options.Audience = "mediasphere-api";
                options.RequireHttpsMetadata = false;
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer = "http://localhost:8080/realms/mediasphere",
                    ValidateIssuer = true
                };
            });
        services.AddAuthorizationBuilder();

        return services;
    }
}