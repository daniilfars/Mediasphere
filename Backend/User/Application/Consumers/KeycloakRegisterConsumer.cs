using Application.Interfaces;
using Domain;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace Application.Consumers;

public class KeycloakRegisterConsumer : IConsumer<KeycloakRegister>
{
    private readonly IUserDbContext _db;

    public KeycloakRegisterConsumer(IUserDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<KeycloakRegister> context)
    {
        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(context.Message));

        context.Message.Details.TryGetValue("username", out var userName);
        Guid id = context.Message.UserId;

        var userExists = await _db.Users.AnyAsync(u => u.Id == id, context.CancellationToken);

        if (userExists)
            return;

        var result = User.Create(id, userName!);
        if (result.IsFailure)
            return;

        _db.Users.Add(result.Value!);
        await _db.SaveChangesAsync(context.CancellationToken);
    }
}
