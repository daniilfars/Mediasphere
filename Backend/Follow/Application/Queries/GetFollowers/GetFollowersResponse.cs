namespace Application.Queries.GetFollowers;

public sealed record GetFollowersResponse(List<FollowerDto> Followers, Guid FollowingId, int TotalCount, int Page, int PageSize);

public sealed record FollowerDto(Guid FollowerId, string FollowerUserName);