namespace Application.Queries.GetPostById;

public sealed record GetPostByIdResponse(Guid Id, Guid AuthorId, string UserName, string Content, long Likes, long Comments, string? ImageUrl, DateTime CreatedAt);