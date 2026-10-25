using MediatR;
using Shared.Domain;
using System.ComponentModel.DataAnnotations;

namespace Application.Queries.GetFollowings;

public sealed record GetFollowingsQuery(Guid UserId,
    [Range(1, int.MaxValue)] int Page = 1,
    [Range(1, 50)] int PageSize = 10)
    : IRequest<Result<GetFollowingsResponse>>;