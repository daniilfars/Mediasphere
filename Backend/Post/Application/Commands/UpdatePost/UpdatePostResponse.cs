namespace Application.Commands.UpdatePost;

public sealed record UpdatePostResponse(Guid Id, Guid AuthorId, string UserName, string Content, long Likes, string? ImageUrl);