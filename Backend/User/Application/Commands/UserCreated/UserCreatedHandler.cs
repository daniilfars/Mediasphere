using Application.Interfaces;
using Domain;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using Shared.Domain;

namespace Application.Commands.UserCreated;

public sealed class UserCreatedHandler : IRequestHandler<UserCreatedCommand, Result<UserCreatedResponse>>, IConsumer<KeycloakRegister>
{
    private readonly IUserDbContext _context;

    public UserCreatedHandler(IUserDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<KeycloakRegister> context)
    {
        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(context.Message));

        context.Message.Details.TryGetValue("username", out var userName);
        Guid id = context.Message.UserId;

        var userExists = await _context.Users.AnyAsync(u => u.Id == id, context.CancellationToken);

        if (userExists)
            return;

        var result = User.Create(id, userName!);
        if (result.IsFailure)
            return;

        _context.Users.Add(result.Value!);
        await _context.SaveChangesAsync(context.CancellationToken);
    }

    public async Task<Result<UserCreatedResponse>> Handle(UserCreatedCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _context.Users.AsNoTracking()
            .Where(u => u.Id == request.Id)
            .Select(u => new UserCreatedResponse(u.Id, u.UserName))
            .FirstOrDefaultAsync(cancellationToken);

        if(existingUser is not null)
            return Result<UserCreatedResponse>.Success(existingUser);

        var result = User.Create(request.Id, request.UserName);
        if (result.IsFailure)
            return Result<UserCreatedResponse>.Failure(result.Error!);

        var user = result.Value!;

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<UserCreatedResponse>.Success(new UserCreatedResponse(user.Id, user.UserName));
    }
}
