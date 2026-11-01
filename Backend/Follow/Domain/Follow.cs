using Shared.Domain;

namespace Domain;

public class Follow
{
    public Guid FollowerId { get; private set; } // Кто подписывается
    public Guid FollowingId { get; private set; } // На кого
    public string FollowerUserName { get; private set; }
    public string FollowingUserName { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Follow(Guid followerId, Guid followingId, string followerUserName, string followingUserName)
    {
        FollowerId = followerId;
        FollowingId = followingId;
        CreatedAt = DateTimeOffset.UtcNow;
        FollowerUserName = followerUserName;
        FollowingUserName = followingUserName;
    }

    public static Result<Follow> Create(Guid followerId, Guid followingId, string followerUserName, string followingUserName)
    {
        if (followerId == Guid.Empty || followingId == Guid.Empty)
            return Result<Follow>.Failure("ID пользователей не могут быть пустыми");

        if (followerId == followingId)
            return Result<Follow>.Failure("Нельзя подписаться на себя");

        if (string.IsNullOrWhiteSpace(followerUserName))
            return Result<Follow>.Failure("Имя подписчика не может быть пустым");

        if (string.IsNullOrWhiteSpace(followingUserName))
            return Result<Follow>.Failure("Имя на кого подписываются не может быть пустым");

        return Result<Follow>.Success(new Follow(followerId, followingId, followerUserName, followingUserName));
    }
}