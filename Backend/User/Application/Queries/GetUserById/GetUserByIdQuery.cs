using MediatR;
using Shared.Domain;

namespace Application.Queries.GetUser;

public sealed record GetUserByIdQuery(Guid UserId) : IRequest<Result<GetUserByIdResponse>>;
