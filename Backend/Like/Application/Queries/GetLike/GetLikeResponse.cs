using Domain;

namespace Application.Queries.GetLike;

public sealed record GetLikeResponse(Guid Id, Guid UserId, LikeTargetType TargetType, Guid ContentId);
