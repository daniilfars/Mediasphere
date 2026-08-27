using Domain;

namespace Application.Commands.CreateLike;

public sealed record CreateLikeResponse(Guid Id, Guid UserId, LikeTargetType TargetType, Guid ContentId);
