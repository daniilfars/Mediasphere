using MediatR;
using Shared.Domain;

namespace Application.Queries.GetFollowers;

public sealed record GetFollowersQuery(Guid UserId, int Page = 1, int PageSize = 10) : IRequest<Result<GetFollowersResponse>>;