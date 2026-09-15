using MediatR;
using Shared.Domain;

namespace Application.Commands.CreateComment;

public sealed record CreateCommentCommand(Guid UserId, Guid PostId, string Content) : IRequest<Result<CreateCommentResponse>>;