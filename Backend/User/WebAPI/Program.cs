using Infrastructure;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using Shared.Observability;

var builder = WebApplication.CreateBuilder(args);
builder.AddObservability("user-api");

builder.Services.AddUserInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(Application.Queries.GetUser.GetUserByIdHandler).Assembly));

builder.Services.AddAppSecurity();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseTraceIdHeader();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    context.Database.Migrate();
}

app.Run();
