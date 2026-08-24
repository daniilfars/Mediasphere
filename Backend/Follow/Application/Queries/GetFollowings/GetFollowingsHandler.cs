using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace Application.Queries.GetFollowings;

public sealed class GetFollowingsHandler : IRequestHandler<GetFollowingsQuery, Result<GetFollowingsResponse>>
{
    private readonly IFollowDbContext _context;

    public GetFollowingsHandler(IFollowDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GetFollowingsResponse>> Handle(GetFollowingsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Follows.AsNoTracking().Where(f => f.FollowerId == request.UserId);

        var totalCount = await query.CountAsync(cancellationToken);

        var followings = await query
            .OrderByDescending(f => f.CreatedAt)
            .ThenBy(f => f.FollowingId)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(f => new FollowingDto(f.FollowingId))
            .ToListAsync(cancellationToken);

        return Result<GetFollowingsResponse>.Success(new GetFollowingsResponse(followings, request.UserId, totalCount, request.Page, request.PageSize));
    }
}
