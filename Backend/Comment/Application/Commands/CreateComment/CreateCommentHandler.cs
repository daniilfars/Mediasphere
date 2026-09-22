using Application.Interfaces;
using Domain;
using MediatR;
using Shared.Domain;

namespace Application.Commands.CreateComment;

public sealed class CreateCommentHandler : IRequestHandler<CreateCommentCommand, Result<CreateCommentResponse>>
{
    private readonly ICommentDbContext _context;

    public CreateCommentHandler(ICommentDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CreateCommentResponse>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var result = Comment.Create(request.AuthorId, request.UserName, request.PostId, request.Content);
        if (result.IsFailure)
            return Result<CreateCommentResponse>.Failure(result.Error!);

        var comment = result.Value!;

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<CreateCommentResponse>.Success(new CreateCommentResponse(comment.Id, comment.AuthorId, comment.UserName, comment.PostId, comment.Content, comment.Likes));
    }
}
