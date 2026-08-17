using MediatR;

namespace Application.Commands.UserCreated;

public sealed record UserCreatedResponse(Guid Id, string UserName);