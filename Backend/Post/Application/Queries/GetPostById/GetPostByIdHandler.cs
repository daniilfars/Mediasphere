using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace Application.Queries.GetPostById;

public sealed class GetPostByIdHandler : IRequestHandler<GetPostByIdQuery, Result<GetPostByIdResponse>>
{
    private readonly IPostDbContext _context;

    public GetPostByIdHandler(IPostDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GetPostByIdResponse>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _context.Posts.AsNoTracking()
            .Where(p => p.Id == request.Id)
            .Select(p => new GetPostByIdResponse(p.Id, p.AuthorId, p.UserName, p.Content, p.ImageUrl))
            .FirstOrDefaultAsync(cancellationToken);

        if (post is null)
            return Result<GetPostByIdResponse>.Failure("Пост не найден");

        return Result<GetPostByIdResponse>.Success(post);
    }
}
