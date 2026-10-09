using Domain;

namespace Application.Queries.GetLike;

public sealed record GetLikeByContentIdResponse(Guid Id, Guid UserId, LikeTargetType TargetType, Guid ContentId);
