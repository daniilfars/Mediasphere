namespace Gateway.Models;

public sealed record CommentsDto(List<CommentResponseDto> Comments, int TotalCount, int Page, int PageSize);

public sealed record CommentResponseDto(Guid Id, Guid AuthorId, string UserName, Guid PostId, string Content, long Likes, bool IsLiked);