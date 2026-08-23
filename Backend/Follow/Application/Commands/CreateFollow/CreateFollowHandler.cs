using Application.Interfaces;
using Domain;
using MediatR;
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
        var follow = Follow.Create(request.FollowerId, request.FollowingId);

        _context.Follows.Add(follow);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<CreateFollowResponse>.Success(new CreateFollowResponse(follow.FollowerId, follow.FollowingId));
    }
}
