namespace Application.Commands.CreateComment;

public sealed record CreateCommentResponse(Guid Id, Guid UserId, Guid PostId, string Content);