using Application.Commands.CreatePost;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Domain;

namespace Application.Commands.UploadPostImage;

public sealed class UploadPostImageHandler : IRequestHandler<UploadPostImageCommand, Result<UploadPostImageResponse>>
{
    private readonly IPostDbContext _context;
    private readonly IImageStorageService _imageStorageService;
    private readonly ILogger<UploadPostImageHandler> _logger;

    public UploadPostImageHandler(IPostDbContext context, IImageStorageService imageStorageService, ILogger<UploadPostImageHandler> logger)
    {
        _context = context;
        _imageStorageService = imageStorageService;
        _logger = logger;
    }

    public async Task<Result<UploadPostImageResponse>> Handle(UploadPostImageCommand request, CancellationToken cancellationToken)
    {
        var extension = request.ContentType switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => null
        };

        if (extension is null)
            return Result<UploadPostImageResponse>.Failure("Неподдерживаемый формат изображения");

        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == request.Id);
        if (post is null)
            return Result<UploadPostImageResponse>.Failure("Пост не найден");

        if (post.AuthorId != request.AuthorId)
            return Result<UploadPostImageResponse>.Failure("Нету прав на обновление поста");

        var oldImageUrl = post.ImageUrl;
        var objectName = $"posts/{post.Id}/{Guid.NewGuid()}{extension}";

        try
        {
            var url = await _imageStorageService.UploadAsync(objectName, request.FileStream, request.ContentType, cancellationToken);
            post.SetImageUrl(url);

            await _context.SaveChangesAsync(cancellationToken);

            if (oldImageUrl != null)
            {
                try
                {
                    var oldObjectName = _imageStorageService.GetObjectNameFromUrl(oldImageUrl);
                    await _imageStorageService.DeleteAsync(oldObjectName, cancellationToken);
                }
                catch (Exception deleteEx)
                {
                    _logger.LogError(deleteEx, "Не удалось удалить старую картинку {OldImageUrl}", oldImageUrl);
                }
            }

            return Result<UploadPostImageResponse>.Success(new UploadPostImageResponse(post.ImageUrl!));
        }
        catch (Exception ex)
        {
            try
            {
                await _imageStorageService.DeleteAsync(objectName, cancellationToken);
            }
            catch (Exception deleteEx)
            {
                _logger.LogError(deleteEx, "Не удалось удалить файл {ObjectName}", objectName);
            }

            _logger.LogError(ex, "Ошибка обновление картинки поста для пользователя {AuthorId}", request.AuthorId);
            return Result<UploadPostImageResponse>.Failure($"Ошибка обновление картинки поста: {ex.Message}");
        }
    }
}