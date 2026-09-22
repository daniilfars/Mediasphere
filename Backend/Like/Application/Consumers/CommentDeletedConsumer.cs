using Domain;
using Application.Interfaces;
using MassTransit;
using Shared.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Application.Consumers;

public class CommentDeletedConsumer : IConsumer<CommentDeleted>
{
    private readonly ILikeDbContext _db;

    public CommentDeletedConsumer(ILikeDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<CommentDeleted> context)
    {
        await _db.Likes.Where(l => l.TargetType == LikeTargetType.Comment && l.ContentId == context.Message.CommentId)
            .ExecuteDeleteAsync(context.CancellationToken);
    }
}
