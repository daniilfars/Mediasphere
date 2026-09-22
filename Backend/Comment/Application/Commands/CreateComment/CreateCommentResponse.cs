namespace Application.Commands.CreateComment;

public sealed record CreateCommentResponse(Guid Id, Guid AuthorId, string UserName, Guid PostId, string Content, long Likes);