using MediatR;
using Shared.Domain;
using System.ComponentModel.DataAnnotations;

namespace Application.Queries.GetFollowers;

public sealed record GetFollowersQuery(Guid UserId,
    [Range(1, int.MaxValue)] int Page = 1,
    [Range(1, 50)] int PageSize = 10)
    : IRequest<Result<GetFollowersResponse>>;