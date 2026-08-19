using MediatR;
using Shared.Domain;

namespace Application.Queries.GetPostById;

public sealed record GetPostByIdQuery(Guid Id) : IRequest<Result<GetPostByIdResponse>>;
