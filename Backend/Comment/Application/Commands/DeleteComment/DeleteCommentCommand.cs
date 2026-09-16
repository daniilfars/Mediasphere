using MediatR;
using Shared.Domain;

namespace Application.Commands.DeleteComment;

public sealed record DeleteCommentCommand(Guid Id, Guid UserId) : IRequest<Result>;