using Domain;
using MediatR;
using Shared.Domain;

namespace Application.Commands.CreateLike;

public sealed record CreateLikeCommand(Guid UserId, LikeTargetType TargetType, Guid ContentId) : IRequest<Result<CreateLikeResponse>>;