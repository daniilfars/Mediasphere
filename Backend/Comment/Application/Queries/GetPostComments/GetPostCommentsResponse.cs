namespace Application.Queries.GetPostComments;

public sealed record GetPostCommentsResponse(List<CommentDto> Comments, int TotalCount, int Page, int PageSize);

public sealed record CommentDto(Guid Id, Guid AuthorId, string UserName, Guid PostId, string Content, long Likes);