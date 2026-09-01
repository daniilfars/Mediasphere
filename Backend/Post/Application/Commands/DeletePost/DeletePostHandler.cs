using Application.Interfaces;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Contracts;
using Shared.Domain;

namespace Application.Commands.DeletePost;

public sealed class DeletePostHandler : IRequestHandler<DeletePostCommand, Result>
{
    private readonly IPostDbContext _context;
    private readonly IImageStorageService _imageStorageService;
    private readonly ILogger<DeletePostHandler> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public DeletePostHandler(IPostDbContext context, IImageStorageService imageStorageService, ILogger<DeletePostHandler> logger, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _imageStorageService = imageStorageService;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == request.Id);
        if (post is null)
            return Result.Failure("Пост не найден");

        if (post.AuthorId != request.UserId)
            return Result.Failure("Нету прав на удаление поста");

        if (post.ImageUrl != null)
        {
            var objectName = _imageStorageService.GetObjectNameFromUrl(post.ImageUrl);
            try
            {
                await _imageStorageService.DeleteAsync(objectName, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось удалить файл {ObjectName}", objectName);
            }
        }

        _context.Posts.Remove(post);

        await _publishEndpoint.Publish<PostDeleted>(new { PostId = post.Id }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}