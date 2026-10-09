using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace Application.Queries.GetLike;

public sealed class GetLikeByContentIdHandler : IRequestHandler<GetLikeByContentIdQuery, Result<GetLikeByContentIdResponse>>
{
    private readonly ILikeDbContext _context;

    public GetLikeByContentIdHandler(ILikeDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GetLikeByContentIdResponse>> Handle(GetLikeByContentIdQuery request, CancellationToken cancellationToken)
    {
        var like = await _context.Likes.AsNoTracking()
            .Where(l => l.UserId == request.UserId && l.TargetType == request.TargetType && l.ContentId == request.ContentId)
            .Select(l => new GetLikeByContentIdResponse(l.Id, l.UserId, l.TargetType, l.ContentId))
            .FirstOrDefaultAsync(cancellationToken);

        if (like is null)
            return Result<GetLikeByContentIdResponse>.Failure("Лайк не найден");

        return Result<GetLikeByContentIdResponse>.Success(like);
    }
}
