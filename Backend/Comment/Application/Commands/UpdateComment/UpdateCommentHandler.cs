using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace Application.Commands.UpdateComment;

public sealed class UpdateCommentHandler : IRequestHandler<UpdateCommentCommand, Result<UpdateCommentResponse>>
{
    private readonly ICommentDbContext _context;

    public UpdateCommentHandler(ICommentDbContext context)
    {
        _context = context;
    }

    public async Task<Result<UpdateCommentResponse>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (comment is null)
            return Result<UpdateCommentResponse>.Failure("Комментарий не найден");

        if (comment.UserId != request.UserId)
            return Result<UpdateCommentResponse>.Failure("Нет доступа к редактированию комментария");

        var result = comment.UpdateContent(request.Content);
        if (result.IsFailure)
            return Result<UpdateCommentResponse>.Failure(result.Error!);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<UpdateCommentResponse>.Success(new UpdateCommentResponse(comment.Id, comment.UserId, comment.PostId, comment.Content));
    }
}
