using MediatR;
using Shared.Domain;

namespace Application.Commands.DeletePost;

public sealed record DeletePostCommand(Guid Id, Guid UserId) : IRequest<Result>;