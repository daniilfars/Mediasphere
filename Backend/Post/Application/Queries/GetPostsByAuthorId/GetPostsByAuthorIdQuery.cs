using MediatR;
using Shared.Domain;

namespace Application.Queries.GetPostsByAuthorId;

public sealed record GetPostsByAuthorIdQuery(Guid AuthorId, int Page = 1, int PageSize = 10) : IRequest<Result<GetPostsByAuthorIdResponse>>;
