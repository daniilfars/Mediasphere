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
        Console.WriteLine("============= УРАА или НЕЕТ? ===================");

        var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == context.Message.PostId, context.CancellationToken);
        if(post is null)
        {
            await context.Publish<ContentNotFound>(new { LikeId = context.Message.LikeId }, context.CancellationToken);
            return;
        }

        post.AddLike();

        await _db.SaveChangesAsync(context.CancellationToken);
    }
}
