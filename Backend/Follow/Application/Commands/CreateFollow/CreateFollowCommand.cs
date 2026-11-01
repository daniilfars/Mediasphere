using MediatR;
using Shared.Domain;

namespace Application.Commands.CreateFollow;

public sealed record CreateFollowCommand(Guid FollowerId, Guid FollowingId, string FollowerUserName, string FollowingUserName) : IRequest<Result<CreateFollowResponse>>;
