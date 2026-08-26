using Shared.Domain;

namespace Domain;

public class Like
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public LikeTargetType TargetType { get; private set; }

    private Like(Guid userId, LikeTargetType targetType)
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        TargetType = targetType;
    }

    public static Result<Like> Create(Guid UserId, LikeTargetType TargetType)
    {
        return Result.Success(new Like(UserId, TargetType));
    }
}