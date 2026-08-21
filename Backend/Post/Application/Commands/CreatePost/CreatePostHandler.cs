using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Domain;

namespace Application.Commands.CreatePost;

public sealed class CreatePostHandler : IRequestHandler<CreatePostCommand, Result<CreatePostResponse>>
{
    private readonly IPostDbContext _context;
    private readonly IImageStorageService _imageStorageService;
    private readonly ILogger<CreatePostHandler> _logger;

    public CreatePostHandler(IPostDbContext context, IImageStorageService imageStorageService, ILogger<CreatePostHandler> logger)
    {
        _context = context;
        _imageStorageService = imageStorageService;
        _logger = logger;
    }

    public async Task<Result<CreatePostResponse>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var extension = request.ContentType switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => null
        };

        if (extension is null)
            return Result<CreatePostResponse>.Failure("Неподдерживаемый формат изображения");

        var result = Post.Create(request.AuthorId, request.UserName, request.Content);
        if (result.IsFailure)
            return Result<CreatePostResponse>.Failure(result.Error!);

        var post = result.Value!;
        var objectName = $"posts/{post.Id}/{Guid.NewGuid()}{extension}";

        try
        {
            var url = await _imageStorageService.UploadAsync(objectName, request.FileStream, request.ContentType, cancellationToken);
            post.SetImageUrl(url);

            _context.Posts.Add(post);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<CreatePostResponse>.Success(new CreatePostResponse(post.Id, post.AuthorId, post.UserName, post.Content, post.Likes, post.ImageUrl));
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

            _logger.LogError(ex, "Ошибка создания поста для пользователя {AuthorId}", request.AuthorId);
            return Result<CreatePostResponse>.Failure($"Ошибка создания поста: {ex.Message}");
        }
    }
}