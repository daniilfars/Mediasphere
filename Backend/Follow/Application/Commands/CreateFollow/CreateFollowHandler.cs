using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace Application.Commands.CreateFollow;

public sealed class CreateFollowHandler : IRequestHandler<CreateFollowCommand, Result<CreateFollowResponse>>
{
    private readonly IFollowDbContext _context;

    public CreateFollowHandler(IFollowDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CreateFollowResponse>> Handle(CreateFollowCommand request, CancellationToken cancellationToken)
    {
        var existFollow = await _context.Follows.AsNoTracking()
            .Where(f => f.FollowerId == request.FollowerId && f.FollowingId == request.FollowingId)
            .Select(f => new CreateFollowResponse(f.FollowerId, f.FollowingId))
            .FirstOrDefaultAsync(cancellationToken);

        if(existFollow is not null)
            return Result<CreateFollowResponse>.Success(existFollow);

        var result = Follow.Create(request.FollowerId, request.FollowingId);
        if (result.IsFailure)
            return Result<CreateFollowResponse>.Failure(result.Error!);

        var follow = result.Value!;

        _context.Follows.Add(follow);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<CreateFollowResponse>.Success(new CreateFollowResponse(follow.FollowerId, follow.FollowingId));
    }
}
