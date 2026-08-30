using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace Application.Commands.DeleteLike;

public sealed class DeleteLikeHandler : IRequestHandler<DeleteLikeCommand, Result>
{
    private readonly ILikeDbContext _context;

    public DeleteLikeHandler(ILikeDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteLikeCommand request, CancellationToken cancellationToken)
    {
        var like = await _context.Likes.FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);
        if (like is null)
            return Result.Failure("Лайк не найден");

        if(like.UserId != request.UserId)
            return Result.Failure("Нету прав на удаление лайка");

        _context.Likes.Remove(like);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}