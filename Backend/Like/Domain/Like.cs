using Shared.Domain;

namespace Domain;

public class Like
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public LikeTargetType TargetType { get; private set; }
    public Guid ContentId { get; private set; }

    private Like(Guid userId, LikeTargetType targetType, Guid contentId)
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        TargetType = targetType;
        ContentId = contentId;
    }

    public static Result<Like> Create(Guid userId, LikeTargetType targetType, Guid contentId)
    {
        if (userId == Guid.Empty || contentId == Guid.Empty)
            return Result<Like>.Failure("ID пользователя или контента не могут быть пустыми");

        return Result<Like>.Success(new Like(userId, targetType, contentId));
    }
}