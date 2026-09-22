using Application.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
namespace Application.Consumers;

public class LikeOnCommentConsumer : IConsumer<LikeOnComment>
{
    private readonly ICommentDbContext _db;

    public LikeOnCommentConsumer(ICommentDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<LikeOnComment> context)
    {
        var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == context.Message.CommentId, context.CancellationToken);
        if(comment is null)
        {
            await context.Publish<ContentNotFound>(new { LikeId = context.Message.LikeId }, context.CancellationToken);
            return;
        }

        comment.AddLike();

        await _db.SaveChangesAsync(context.CancellationToken);
    }
}