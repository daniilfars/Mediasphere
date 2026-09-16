using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace Application.Commands.DeleteComment;

public sealed class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand, Result>
{
    private readonly ICommentDbContext _context;

    public DeleteCommentHandler(ICommentDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (comment is null)
            return Result.Failure("Комментарий не найден");

        if (comment.UserId != request.UserId)
            return Result.Failure("Нет доступа к удалению комментария");

        _context.Comments.Remove(comment);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
