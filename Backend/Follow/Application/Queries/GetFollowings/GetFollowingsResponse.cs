namespace Application.Queries.GetFollowings;

public sealed record GetFollowingsResponse(List<FollowingDto> followers, Guid FollowerId, int TotalCount, int Page, int PageSize);

public sealed record FollowingDto(Guid FollowingId);
