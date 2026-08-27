using Shared.Domain;

namespace Domain;

public class Follow
{
    public Guid FollowerId { get; private set; } // Кто подписывается
    public Guid FollowingId { get; private set; } // На кого
    public DateTimeOffset CreatedAt { get; private set; }

    private Follow(Guid followerId, Guid followingId)
    {
        FollowerId = followerId;
        FollowingId = followingId;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Result<Follow> Create(Guid followerId, Guid followingId)
    {
        if (followerId == Guid.Empty || followingId == Guid.Empty)
            return Result<Follow>.Failure("ID пользователей не могут быть пустыми");

        if (followerId == followingId)
            return Result<Follow>.Failure("Нельзя подписаться на себя");

        return Result<Follow>.Success(new Follow(followerId, followingId));
    }
}