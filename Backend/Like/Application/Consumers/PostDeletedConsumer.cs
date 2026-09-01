using Application.Interfaces;
using Domain;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace Application.Consumers;

public class PostDeletedConsumer : IConsumer<PostDeleted>
{
    private readonly ILikeDbContext _db;

    public PostDeletedConsumer(ILikeDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<PostDeleted> context)
    {
        await _db.Likes.Where(l => l.TargetType == LikeTargetType.Post && l.ContentId == context.Message.PostId)
            .ExecuteDeleteAsync(context.CancellationToken);
    }
}