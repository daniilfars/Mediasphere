using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace Application.Queries.GetPostsByAuthorId;

public sealed class GetPostsByAuthorIdHandler : IRequestHandler<GetPostsByAuthorIdQuery, Result<GetPostsByAuthorIdResponse>>
{
    private readonly IPostDbContext _context;

    public GetPostsByAuthorIdHandler(IPostDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GetPostsByAuthorIdResponse>> Handle(GetPostsByAuthorIdQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Posts.AsNoTracking()
            .Where(p => p.AuthorId == request.AuthorId);

        var totalCount = await query.CountAsync(cancellationToken);

        var posts = await query
            .OrderByDescending(p => p.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new GetPostByAuthorIdDto(p.Id, p.AuthorId, p.UserName, p.Content, p.Likes, p.Comments, p.ImageUrl, p.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<GetPostsByAuthorIdResponse>.Success(new GetPostsByAuthorIdResponse(posts, totalCount, request.Page, request.PageSize));
    }
}