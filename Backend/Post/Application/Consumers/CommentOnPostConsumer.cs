using Application.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace Application.Consumers;
public class CommentOnPostConsumer : IConsumer<CommentOnPost>
{
    private readonly IPostDbContext _db;

    public CommentOnPostConsumer(IPostDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<CommentOnPost> context)
    {
        var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == context.Message.PostId, context.CancellationToken);
        if (post is null)
        {
            await context.Publish<CommentedPostNotFound>(new { CommentId = context.Message.CommentId }, context.CancellationToken);
            return;
        }

        post.AddComment();

        await _db.SaveChangesAsync(context.CancellationToken);
    }
}