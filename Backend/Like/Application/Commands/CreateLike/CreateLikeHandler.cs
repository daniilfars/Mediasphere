using Application.Interfaces;
using Domain;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Shared.Contracts;
using Shared.Domain;

namespace Application.Commands.CreateLike;

public sealed class CreateLikeHandler : IRequestHandler<CreateLikeCommand, Result<CreateLikeResponse>>
{
    private readonly ILikeDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateLikeHandler(ILikeDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<CreateLikeResponse>> Handle(CreateLikeCommand request, CancellationToken cancellationToken)
    {
        var existLike = await FindExistingAsync(request, cancellationToken);

        if(existLike is not null)
            return Result<CreateLikeResponse>.Success(existLike);

        var result = Like.Create(request.UserId, request.TargetType, request.ContentId);
        if (result.IsFailure)
            return Result<CreateLikeResponse>.Failure(result.Error!);

        var like = result.Value!;

        _context.Likes.Add(like);

        if (like.TargetType == LikeTargetType.Post)
            await _publishEndpoint.Publish<LikeOnPost>(new { LikeId = like.Id, PostId = like.ContentId }, cancellationToken);
        if (like.TargetType == LikeTargetType.Comment)
            await _publishEndpoint.Publish<LikeOnComment>(new { LikeId = like.Id, CommentId = like.ContentId }, cancellationToken);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            var concurrent = await FindExistingAsync(request, cancellationToken);
            if (concurrent is not null)
                return Result<CreateLikeResponse>.Success(concurrent);

            throw;
        }

        return Result<CreateLikeResponse>.Success(new CreateLikeResponse(like.Id, like.UserId, like.TargetType, like.ContentId));
    }

    private Task<CreateLikeResponse?> FindExistingAsync(CreateLikeCommand request, CancellationToken cancellationToken) =>
        _context.Likes.AsNoTracking()
            .Where(l => l.UserId == request.UserId && l.TargetType == request.TargetType && l.ContentId == request.ContentId)
            .Select(l => new CreateLikeResponse(l.Id, l.UserId, l.TargetType, l.ContentId))
            .FirstOrDefaultAsync(cancellationToken);
}
