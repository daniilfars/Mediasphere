namespace Gateway.Models;

public sealed record FeedDto(List<PostDto> Posts, int TotalCount, int Page, int PageSize);

public sealed record PostDto(Guid Id, Guid AuthorId, string UserName, string Content, long Likes, string? ImageUrl, DateTime CreatedAt, bool IsLiked);