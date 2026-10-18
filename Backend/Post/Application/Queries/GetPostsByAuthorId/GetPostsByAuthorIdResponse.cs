namespace Application.Queries.GetPostsByAuthorId;

public sealed record GetPostsByAuthorIdResponse(List<GetPostByAuthorIdDto> Posts, int TotalCount, int Page, int PageSize);

public sealed record GetPostByAuthorIdDto(Guid Id, Guid AuthorId, string UserName, string Content, long Likes, long Comments, string? ImageUrl, DateTime CreatedAt);