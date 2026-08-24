using MediatR;
using Shared.Domain;

namespace Application.Queries.GetFollowings;

public sealed record GetFollowingsQuery(Guid UserId, int Page = 1, int PageSize = 10) : IRequest<Result<GetFollowingsResponse>>;