using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace Application.Queries.GetPostComments;

public sealed class GetPostCommentsHandler : IRequestHandler<GetPostCommentsQuery, Result<GetPostCommentsResponse>>
{
    private readonly ICommentDbContext _context;

    public GetPostCommentsHandler(ICommentDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GetPostCommentsResponse>> Handle(GetPostCommentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Comments.AsNoTracking()
            .Where(c => c.PostId == request.PostId);

        var totalCount = await query.CountAsync();

        var comments = await query.Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CommentDto(c.Id, c.AuthorId, c.UserName, c.PostId, c.Content, c.Likes))
            .ToListAsync(cancellationToken);

        return Result<GetPostCommentsResponse>.Success(new GetPostCommentsResponse(comments, totalCount, request.Page, request.PageSize));
    }
}