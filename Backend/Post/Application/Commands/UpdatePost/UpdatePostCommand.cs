using MediatR;
using Shared.Domain;

namespace Application.Commands.UpdatePost;

public sealed record UpdatePostCommand(Guid Id, Guid UserId, string? Content = null) : IRequest<Result<UpdatePostResponse>>;