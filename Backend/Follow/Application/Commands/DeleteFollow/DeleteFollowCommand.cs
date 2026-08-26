using MediatR;
using Shared.Domain;

namespace Application.Commands.DeleteFollow;

public sealed record DeleteFollowCommand(Guid FollowerId, Guid FollowingId) : IRequest<Result>;
