using Application.Interfaces;
using Domain;
using MassTransit;
using MediatR;
using Shared.Contracts;
using Shared.Domain;

namespace Application.Commands.CreateComment;

public sealed class CreateCommentHandler : IRequestHandler<CreateCommentCommand, Result<CreateCommentResponse>>
{
    private readonly ICommentDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateCommentHandler(ICommentDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<CreateCommentResponse>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var result = Comment.Create(request.AuthorId, request.UserName, request.PostId, request.Content);
        if (result.IsFailure)
            return Result<CreateCommentResponse>.Failure(result.Error!);

        var comment = result.Value!;

        _context.Comments.Add(comment);

        await _publishEndpoint.Publish<CommentOnPost>(new { CommentId = comment.Id, PostId = comment.PostId }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<CreateCommentResponse>.Success(new CreateCommentResponse(comment.Id, comment.AuthorId, comment.UserName, comment.PostId, comment.Content, comment.Likes));
    }
}
