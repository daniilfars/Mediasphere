namespace Application.Commands.CreatePost;

public sealed record CreatePostResponse(Guid Id, Guid AuthorId, string UserName, string Content, string? ImageUrl);
