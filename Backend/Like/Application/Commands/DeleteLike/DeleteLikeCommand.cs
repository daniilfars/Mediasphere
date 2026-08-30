using MediatR;
using Shared.Domain;

namespace Application.Commands.DeleteLike;

public sealed record DeleteLikeCommand(Guid Id, Guid UserId) : IRequest<Result>;