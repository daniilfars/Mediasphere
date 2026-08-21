using Shared.Domain;
using MediatR;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands.UpdatePost;

public sealed class UpdatePostHandler : IRequestHandler<UpdatePostCommand, Result<UpdatePostResponse>>
{
    private readonly IPostDbContext _context;

    public UpdatePostHandler(IPostDbContext context)
    {
        _context = context;
    }

    public async Task<Result<UpdatePostResponse>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (post is null)
            return Result<UpdatePostResponse>.Failure("Пост не найден");

        if(post.AuthorId != request.UserId)
            return Result<UpdatePostResponse>.Failure("Нету прав на обновление поста");

        if (!string.IsNullOrWhiteSpace(request.Content))
        {
            var result = post.UpdateContent(request.Content);
            if(result.IsFailure)
                return Result<UpdatePostResponse>.Failure(result.Error!);
        }
         
        await _context.SaveChangesAsync(cancellationToken);

        return Result<UpdatePostResponse>.Success(new UpdatePostResponse(post.Id, post.AuthorId, post.UserName, post.Content, post.Likes, post.ImageUrl));
    }
}
