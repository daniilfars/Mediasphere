using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace Application.Queries.GetFollowers;

public sealed class GetFollowersHandler : IRequestHandler<GetFollowersQuery, Result<GetFollowersResponse>>
{
    private readonly IFollowDbContext _context;

    public GetFollowersHandler(IFollowDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GetFollowersResponse>> Handle(GetFollowersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Follows.AsNoTracking().Where(f => f.FollowingId == request.UserId);

        var totalCount = await query.CountAsync(cancellationToken);

        var followers = await query
            .OrderByDescending(f => f.CreatedAt)
            .ThenBy(f => f.FollowerId)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(f => new FollowerDto(f.FollowerId))
            .ToListAsync(cancellationToken);

        return Result<GetFollowersResponse>.Success(new GetFollowersResponse(followers, request.UserId, totalCount, request.Page, request.PageSize));
    }
}
