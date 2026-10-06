using Application.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace Application.Consumers;

public class CommentOnPostDeletedConsumer : IConsumer<CommentOnPostDeleted>
{
    private readonly IPostDbContext _db;

    public CommentOnPostDeletedConsumer(IPostDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<CommentOnPostDeleted> context)
    {
        var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == context.Message.PostId, context.CancellationToken);
        if (post is null)
            return;

        post.DeleteComment();
        await _db.SaveChangesAsync(context.CancellationToken);
    }
}