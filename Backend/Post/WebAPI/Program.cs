using Infrastructure;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using Shared.Observability;

var builder = WebApplication.CreateBuilder(args);
builder.AddObservability("post-api");

builder.Services.AddPostInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(Application.Commands.CreatePost.CreatePostHandler).Assembly));

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
    var context = scope.ServiceProvider.GetRequiredService<PostDbContext>();
    context.Database.Migrate();
}

app.Run();