using Application.Interfaces;
using Domain;
using MediatR;
using Shared.Domain;

namespace Application.Commands.UserCreated;

public sealed class UserCreatedHandler : IRequestHandler<UserCreatedCommand, Result<UserCreatedResponse>>
{
    private readonly IUserDbContext _context;

    public UserCreatedHandler(IUserDbContext context)
    {
        _context = context;
    }

    public async Task<Result<UserCreatedResponse>> Handle(UserCreatedCommand request, CancellationToken cancellationToken)
    {
        var result = User.Create(request.Id, request.UserName);
        if (result.IsFailure)
            return Result<UserCreatedResponse>.Failure(result.Error!);

        var user = result.Value!;

        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<UserCreatedResponse>.Success(new UserCreatedResponse(user.Id, user.UserName));
    }
}
