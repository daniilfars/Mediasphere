using Application.Interfaces;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using Shared.Domain;

namespace Application.Commands.DeleteComment;

public sealed class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand, Result>
{
    private readonly ICommentDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public DeleteCommentHandler(ICommentDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (comment is null)
            return Result.Failure("Комментарий не найден");

        if (comment.AuthorId != request.AuthorId)
            return Result.Failure("Нет доступа к удалению комментария");

        _context.Comments.Remove(comment);

        await _publishEndpoint.Publish<CommentDeleted>(new { CommentId = comment.Id }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
