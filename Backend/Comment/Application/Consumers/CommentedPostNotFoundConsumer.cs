using Application.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace Application.Consumers;

public class CommentedPostNotFoundConsumer : IConsumer<CommentedPostNotFound>
{
    private readonly ICommentDbContext _db;

    public CommentedPostNotFoundConsumer(ICommentDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<CommentedPostNotFound> context)
    {
        var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == context.Message.CommentId, context.CancellationToken);
        if (comment is null)
            return;

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync(context.CancellationToken);
    }
}
