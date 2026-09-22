namespace Application.Commands.UpdateComment;

public sealed record UpdateCommentResponse(Guid Id, Guid AuthorId, string UserName, Guid PostId, string Content, long Likes);
