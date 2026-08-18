using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace Application.Queries.GetUser;

public sealed class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, Result<GetUserByIdResponse>>
{
    private readonly IUserDbContext _context;

    public GetUserByIdHandler(IUserDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GetUserByIdResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.AsNoTracking()
            .Where(u => u.Id == request.UserId)
            .Select(u => new GetUserByIdResponse(u.Id, u.UserName))
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            return Result<GetUserByIdResponse>.Failure("Пользователь не найден");

        return Result<GetUserByIdResponse>.Success(user);
    }
}
