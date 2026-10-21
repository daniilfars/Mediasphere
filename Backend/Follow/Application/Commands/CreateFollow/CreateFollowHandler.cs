using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Shared.Domain;

namespace Application.Commands.CreateFollow;

public sealed class CreateFollowHandler : IRequestHandler<CreateFollowCommand, Result<CreateFollowResponse>>
{
    private readonly IFollowDbContext _context;

    public CreateFollowHandler(IFollowDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CreateFollowResponse>> Handle(CreateFollowCommand request, CancellationToken cancellationToken)
    {
        var existFollow = await FindExistsAsync(request, cancellationToken);

        if (existFollow is not null)
            return Result<CreateFollowResponse>.Success(existFollow);

        var result = Follow.Create(request.FollowerId, request.FollowingId);
        if (result.IsFailure)
            return Result<CreateFollowResponse>.Failure(result.Error!);

        var follow = result.Value!;

        _context.Follows.Add(follow);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch(DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            var concurrent = await FindExistsAsync(request, cancellationToken);
            if (concurrent is not null)
                return Result<CreateFollowResponse>.Success(concurrent);

            throw;
        }

        return Result<CreateFollowResponse>.Success(new CreateFollowResponse(follow.FollowerId, follow.FollowingId));
    }

    private Task<CreateFollowResponse?> FindExistsAsync(CreateFollowCommand request, CancellationToken cancellationToken) =>
        _context.Follows.AsNoTracking()
            .Where(f => f.FollowerId == request.FollowerId && f.FollowingId == request.FollowingId)
            .Select(f => new CreateFollowResponse(f.FollowerId, f.FollowingId))
            .FirstOrDefaultAsync(cancellationToken);
}
