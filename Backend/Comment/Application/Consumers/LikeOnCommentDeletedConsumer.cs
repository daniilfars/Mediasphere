using Application.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace Application.Consumers;

public class LikeOnCommentDeletedConsumer : IConsumer<LikeOnCommentDeleted>
{
    private readonly ICommentDbContext _db;

    public LikeOnCommentDeletedConsumer(ICommentDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<LikeOnCommentDeleted> context)
    {
        var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == context.Message.CommentId, context.CancellationToken);
        if (comment is null)
            return;

        comment.DeleteLike();
        await _db.SaveChangesAsync(context.CancellationToken);
    }
}
