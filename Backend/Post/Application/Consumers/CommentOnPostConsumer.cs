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
        var updated = await _db.Posts
            .Where(p => p.Id == context.Message.PostId)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Comments, p => p.Comments + 1), context.CancellationToken);

        if (updated == 0)
            await context.Publish<CommentedPostNotFound>(new { CommentId = context.Message.CommentId }, context.CancellationToken);
    }
}