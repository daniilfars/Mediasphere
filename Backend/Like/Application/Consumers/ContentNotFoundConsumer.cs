using Application.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace Application.Consumers;

public class ContentNotFoundConsumer : IConsumer<ContentNotFound>
{
    private readonly ILikeDbContext _db;

    public ContentNotFoundConsumer(ILikeDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<ContentNotFound> context)
    {
        var like = await _db.Likes.FirstOrDefaultAsync(l => l.Id == context.Message.LikeId, context.CancellationToken);
        if (like is null)
            return;

        _db.Likes.Remove(like);
        await _db.SaveChangesAsync(context.CancellationToken);
    }
}
