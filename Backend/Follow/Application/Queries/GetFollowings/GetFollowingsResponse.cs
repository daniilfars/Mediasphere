namespace Application.Queries.GetFollowings;

public sealed record GetFollowingsResponse(List<FollowingDto> Followers, Guid FollowerId, int TotalCount, int Page, int PageSize);

public sealed record FollowingDto(Guid FollowingId, string FollowerUserName);
