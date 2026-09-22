using MediatR;
using Shared.Domain;

namespace Application.Commands.UpdateComment;

public sealed record UpdateCommentCommand(Guid Id, Guid AuthorId, string? Content) : IRequest<Result<UpdateCommentResponse>>;