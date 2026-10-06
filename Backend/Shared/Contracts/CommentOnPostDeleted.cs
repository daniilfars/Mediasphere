namespace Shared.Contracts;

public interface CommentOnPostDeleted
{
    Guid PostId { get; }
}