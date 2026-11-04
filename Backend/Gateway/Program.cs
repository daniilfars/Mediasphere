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

app.MapGet("api/feed", async (Guid? authorId, int? page, int? pageSize, System.Security.Claims.ClaimsPrincipal user, IHttpClientFactory httpClientFactory, LikeService.LikeServiceClient likeClient, ILogger<Program> logger) =>
{
    var httpClient = httpClientFactory.CreateClient();
    GetPostsResponse? response;
    try
    {
        if (authorId is not null && authorId != Guid.Empty)
            response = await httpClient.GetFromJsonAsync<GetPostsResponse>($"http://post-api:8080/api/post/by-author?authorId={authorId}&Page={page ?? 1}&PageSize={pageSize ?? 10}");
        else
            response = await httpClient.GetFromJsonAsync<GetPostsResponse>($"http://post-api:8080/api/post?Page={page ?? 1}&PageSize={pageSize ?? 10}");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ошибка при запросе к post-api");
        return Results.Problem("Не удалось получить посты", statusCode: 502);
    }

    if (response is null || response.Posts == null || response.Posts.Count == 0)
        return Results.NoContent();

    var userId = user.FindFirst("sub")?.Value;

    var likes = new Dictionary<string, bool>();

    if (!string.IsNullOrWhiteSpace(userId))
    {
        var request = new CheckLikesRequest { UserId = userId, TargetType = TargetType.Post };
        request.ContentIds.AddRange(response.Posts.Select(x => x.Id.ToString()));

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
        response.Posts.ConvertAll(p => new PostDto(p.Id, p.AuthorId, p.UserName, p.Content, p.Likes, p.Comments, p.ImageUrl, p.CreatedAt, likes.TryGetValue(p.Id.ToString(), out var isLiked) && isLiked)),
        response.TotalCount, response.Page, response.PageSize
    ));
});

app.MapGet("api/comments", async (Guid postId, int? page, int? pageSize, System.Security.Claims.ClaimsPrincipal user, IHttpClientFactory httpClientFactory, LikeService.LikeServiceClient likeClient, ILogger<Program> logger) =>
{
    var httpClient = httpClientFactory.CreateClient();
    GetPostCommentsResponse? response;
    try
    {
        response = await httpClient.GetFromJsonAsync<GetPostCommentsResponse>($"http://comment-api:8080/api/comment?postId={postId}&page={page ?? 1}&pageSize={pageSize ?? 10}");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ошибка при запросе к comment-api");
        return Results.Problem("Не удалось получить комментарии поста", statusCode: 502);
    }

    if (response is null || response.Comments == null || response.Comments.Count == 0)
        return Results.NoContent();

    var userId = user.FindFirst("sub")?.Value;

    var likes = new Dictionary<string, bool>();

    if (!string.IsNullOrWhiteSpace(userId))
    {
        var request = new CheckLikesRequest { UserId = userId, TargetType = TargetType.Comment };
        request.ContentIds.AddRange(response.Comments.Select(x => x.Id.ToString()));

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

    return Results.Ok(new CommentsDto(
        response.Comments.ConvertAll(p => new CommentResponseDto(p.Id, p.AuthorId, p.UserName, p.PostId, p.Content, p.Likes, likes.TryGetValue(p.Id.ToString(), out var isLiked) && isLiked)),
        response.TotalCount, response.Page, response.PageSize
    ));
});

app.MapReverseProxy();

app.Run();