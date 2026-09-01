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
        var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == context.Message.PostId, context.CancellationToken);
        if (post is null) return;

        post.DeleteLike();

        await _db.SaveChangesAsync(context.CancellationToken);
    }
}