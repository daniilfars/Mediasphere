namespace Application.Queries.GetUser;

public sealed record GetUserByIdResponse(Guid UserId, string UserName);