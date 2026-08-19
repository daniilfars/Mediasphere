namespace Application.Queries.GetPostById;

public sealed record GetPostByIdResponse(Guid Id, Guid AuthorId, string UserName, string Content, string? ImageUrl);