using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace Application.Queries.GetPosts;

public sealed class GetPostsHandler : IRequestHandler<GetPostsQuery, Result<GetPostsResponse>>
{
    private readonly IPostDbContext _context;

    public GetPostsHandler(IPostDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GetPostsResponse>> Handle(GetPostsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Posts.AsNoTracking();
        
        var totalCount = await query.CountAsync(cancellationToken);

        var posts = await query
            .OrderByDescending(p => p.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PostDto(p.Id, p.AuthorId, p.UserName, p.Content, p.ImageUrl))
            .ToListAsync(cancellationToken);

        return Result<GetPostsResponse>.Success(new GetPostsResponse(posts, totalCount, request.Page, request.PageSize));
    }
}
