using Gateway.Models;
using LikeGrpc;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppSecurity();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:63250")
                .AllowAnyHeader()
                .AllowAnyMethod();
    });
});

builder.Services.AddHttpClient();

builder.Services.AddGrpcClient<LikeService.LikeServiceClient>(o =>
{
    o.Address = new Uri("http://like-api:5004");
});

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("api/feed", async (int? page, int? pageSize, System.Security.Claims.ClaimsPrincipal user, IHttpClientFactory httpClientFactory, LikeService.LikeServiceClient likeClient, ILogger<Program> logger) =>
{
    var httpClient = httpClientFactory.CreateClient();
    var response = await httpClient.GetFromJsonAsync<GetPostsResponse>($"http://post-api:8080/api/post?Page={page ?? 1}&PageSize={pageSize ?? 10}");
    if(response is null)
        return Results.NoContent();

    var userId = user.FindFirst("sub")?.Value;

    var likes = new Dictionary<string, bool>();

    if (!string.IsNullOrWhiteSpace(userId))
    {
        var postsIds = response.posts.ConvertAll(x => x.Id.ToString());

        var request = new CheckLikesRequest { UserId = userId, TargetType = TargetType.Post };
        request.ContentIds.AddRange(postsIds);

        try
        {
            var res = await likeClient.CheckLikesAsync(request);
            likes = res.Results.ToDictionary(l => l.ContentId, l => l.IsLiked);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Не удалось получить лайки на постах через gRPC");
        }
    }

    return Results.Ok(new FeedDto(
        response.posts.ConvertAll(p => new PostDto(p.Id, p.AuthorId, p.UserName, p.Content, p.Likes, p.Comments, p.ImageUrl, p.CreatedAt, likes.TryGetValue(p.Id.ToString(), out var isLiked) && isLiked)),
        response.TotalCount, response.Page, response.PageSize
    ));
});

app.MapReverseProxy();

app.Run();