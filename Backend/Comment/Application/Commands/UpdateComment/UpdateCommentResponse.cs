namespace Application.Commands.UpdateComment;

public sealed record UpdateCommentResponse(Guid Id, Guid UserId, Guid PostId, string Content);
