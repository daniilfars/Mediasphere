namespace Domain;

public class Follow
{
    public Guid FollowerId { get; private set; } // Кто подписывается
    public Guid FollowingId { get; private set; } // На кого

    private Follow(Guid followerId, Guid followingId)
    {
        FollowerId = followerId;
        FollowingId = followingId;
    }

    public static Follow Create(Guid followerId, Guid followingId)
    {
        return new Follow(followerId, followingId);
    }
}