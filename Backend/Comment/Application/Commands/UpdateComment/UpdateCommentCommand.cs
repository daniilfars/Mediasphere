using MediatR;
using Shared.Domain;

namespace Application.Commands.UpdateComment;

public sealed record UpdateCommentCommand(Guid Id, Guid UserId, string? Content) : IRequest<Result<UpdateCommentResponse>>;