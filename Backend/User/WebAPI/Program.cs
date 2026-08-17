using Infrastructure;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddUserInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(Application.Commands.UserCreated.UserCreatedHandler).Assembly));

builder.Services.AddAuthentication()
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
builder.Services.AddAuthorizationBuilder();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/user-info", (HttpContext context) =>
{
    var user = context.User;
    var allClaims = user.Claims.Select(c => new { c.Type, c.Value }).ToList();

    var userId = user.FindFirst("sub")?.Value;
    var username = user.FindFirst("preferred_username")?.Value;
    var email = user.FindFirst("email")?.Value;
    var roles = user.FindAll("role").Select(r => r.Value).ToList();

    return Results.Ok(new
    {
        Id = userId,
        Username = username,
        Email = email,
        _debugTotalClaimsCount = allClaims.Count,
        _debugAllClaims = allClaims,
        roles = roles
    });
})
.RequireAuthorization();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    context.Database.Migrate();
}

app.Run();
