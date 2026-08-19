namespace Application.Queries.GetPosts;

public sealed record GetPostsResponse(List<PostDto> Posts, int TotalCount, int Page, int PageSize);

public sealed record PostDto(Guid Id, Guid AuthorId, string UserName, string Content, string? ImageUrl);
