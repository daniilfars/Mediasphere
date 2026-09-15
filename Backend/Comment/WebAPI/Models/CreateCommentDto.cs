namespace WebAPI.Models;

public sealed record CreateCommentDto(Guid PostId, string Content);
