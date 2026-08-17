using MediatR;
using Shared.Domain;

namespace Application.Commands.UserCreated;

public sealed record UserCreatedCommand(Guid Id, string UserName) : IRequest<Result<UserCreatedResponse>>;