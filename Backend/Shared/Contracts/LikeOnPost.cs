namespace Shared.Contracts;

public interface LikeOnPost
{
    Guid LikeId { get; }
    Guid PostId { get; }
}
