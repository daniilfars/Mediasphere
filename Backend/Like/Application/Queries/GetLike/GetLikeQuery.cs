using Domain;
using MediatR;
using Shared.Domain;

namespace Application.Queries.GetLike;

public sealed record GetLikeQuery(Guid UserId, LikeTargetType TargetType, Guid ContentId) : IRequest<Result<GetLikeResponse>>;