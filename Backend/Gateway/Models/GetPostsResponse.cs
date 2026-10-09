namespace Gateway.Models;

public sealed record class GetPostsResponse(List<GetPostDto> posts, int TotalCount, int Page, int PageSize);

public sealed record class GetPostDto(Guid Id, Guid AuthorId, string UserName, string Content, long Likes, long Comments, string? ImageUrl, DateTime CreatedAt);