using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace Application.Commands.DeleteFollow;

public sealed class DeleteFollowHandler : IRequestHandler<DeleteFollowCommand, Result>
{
    private readonly IFollowDbContext _context;

    public DeleteFollowHandler(IFollowDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteFollowCommand request, CancellationToken cancellationToken)
    {
        var follow = await _context.Follows.FirstOrDefaultAsync(f => f.FollowerId == request.FollowerId && f.FollowingId == request.FollowingId, cancellationToken);
        if (follow is null)
            return Result.Failure("Подписка/Подписчик не найден");

        _context.Follows.Remove(follow);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
