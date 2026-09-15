namespace Application.Queries.GetPostComments;

public sealed record GetPostCommentsResponse(List<CommentDto> Comments, int TotalCount, int Page, int PageSize);

public sealed record CommentDto(Guid Id, Guid UserId, Guid PostId, string Content);