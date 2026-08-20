namespace WebAPI.Models;

public sealed record UpdatePostRequest(Guid Id, string? Content = null);
