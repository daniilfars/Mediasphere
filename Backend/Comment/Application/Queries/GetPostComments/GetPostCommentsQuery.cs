using MediatR;
using Shared.Domain;

namespace Application.Queries.GetPostComments;

public sealed record GetPostCommentsQuery(Guid PostId, int Page = 1, int PageSize = 10) : IRequest<Result<GetPostCommentsResponse>>;