using Application.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace Application.Consumers;

public class LikeOnPostConsumer : IConsumer<LikeOnPost>
{
    private readonly IPostDbContext _db;

    public LikeOnPostConsumer(IPostDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<LikeOnPost> context)
    {
        var updated = await _db.Posts
            .Where(p => p.Id == context.Message.PostId)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Likes, p => p.Likes + 1), context.CancellationToken);

        if (updated == 0)
            await context.Publish<LikedContentNotFound>(new { LikeId = context.Message.LikeId }, context.CancellationToken);
    }
}