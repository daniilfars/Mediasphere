using Domain;
using MediatR;
using Shared.Domain;

namespace Application.Commands.DeleteLikeByContentId;

public sealed record DeleteLikeByContentCommand(Guid UserId, LikeTargetType TargetType, Guid ContentId) : IRequest<Result>;