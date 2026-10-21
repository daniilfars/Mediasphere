using Application.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace Application.Consumers;

public class LikeOnPostDeletedConsumer : IConsumer<LikeOnPostDeleted>
{
    private readonly IPostDbContext _db;

    public LikeOnPostDeletedConsumer(IPostDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<LikeOnPostDeleted> context)
    {
        await _db.Posts
            .Where(p => p.Id == context.Message.PostId && p.Likes > 0)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Likes, p => p.Likes - 1), context.CancellationToken);
    }
}