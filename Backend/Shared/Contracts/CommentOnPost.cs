namespace Shared.Contracts;

public interface CommentOnPost
{
    Guid CommentId { get; }
    Guid PostId { get; }
}