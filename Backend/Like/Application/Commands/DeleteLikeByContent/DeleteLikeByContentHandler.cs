using Application.Interfaces;
using Domain;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using Shared.Domain;

namespace Application.Commands.DeleteLikeByContentId;

public sealed class DeleteLikeByContentHandler : IRequestHandler<DeleteLikeByContentCommand, Result>
{
    private readonly ILikeDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public DeleteLikeByContentHandler(ILikeDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result> Handle(DeleteLikeByContentCommand request, CancellationToken cancellationToken)
    {
        var like = await _context.Likes.FirstOrDefaultAsync(l => l.UserId == request.UserId && l.TargetType == request.TargetType && l.ContentId == request.ContentId, cancellationToken);
        if (like is null)
            return Result.Failure("Лайк не найден");

        _context.Likes.Remove(like);

        if (like.TargetType == LikeTargetType.Post)
            await _publishEndpoint.Publish<LikeOnPostDeleted>(new { PostId = like.ContentId }, cancellationToken);
        if (like.TargetType == LikeTargetType.Comment)
            await _publishEndpoint.Publish<LikeOnCommentDeleted>(new { CommentId = like.ContentId });

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
