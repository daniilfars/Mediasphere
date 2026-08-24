namespace Application.Queries.GetFollowers;

public sealed record GetFollowersResponse(List<FollowerDto> followers, Guid FollowingId, int TotalCount, int Page, int PageSize);

public sealed record FollowerDto(Guid FollowerId);