using Application.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace Application.Consumers;

public class DeleteCommentsPostDeletedConsumer : IConsumer<PostDeleted>
{
    private readonly ICommentDbContext _db;

    public DeleteCommentsPostDeletedConsumer(ICommentDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<PostDeleted> context)
    {
        var comments = await _db.Comments.Where(c => c.PostId == context.Message.PostId)
            .ExecuteDeleteAsync(context.CancellationToken);
    }
}
