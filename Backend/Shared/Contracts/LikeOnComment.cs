namespace Shared.Contracts;

public interface LikeOnComment
{
    Guid LikeId { get; }
    Guid CommentId { get; }
}