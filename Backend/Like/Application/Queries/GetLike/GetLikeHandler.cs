using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace Application.Queries.GetLike;

public sealed class GetLikeHandler : IRequestHandler<GetLikeQuery, Result<GetLikeResponse>>
{
    private readonly ILikeDbContext _context;

    public GetLikeHandler(ILikeDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GetLikeResponse>> Handle(GetLikeQuery request, CancellationToken cancellationToken)
    {
        var like = await _context.Likes.AsNoTracking()
            .Where(l => l.UserId == request.UserId && l.TargetType == request.TargetType && l.ContentId == request.ContentId)
            .Select(l => new GetLikeResponse(l.Id, l.UserId, l.TargetType, l.ContentId))
            .FirstOrDefaultAsync(cancellationToken);

        if (like is null)
            return Result<GetLikeResponse>.Failure("Лайк не найден");

        return Result<GetLikeResponse>.Success(like);
    }
}
