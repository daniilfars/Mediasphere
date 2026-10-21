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
        await _db.Posts
            .Where(p => p.Id == context.Message.PostId && p.Comments > 0)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Comments, p => p.Comments - 1), context.CancellationToken);
    }
}