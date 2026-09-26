namespace Shared.Contracts;

public interface KeycloakRegister
{
    Guid UserId { get; }
    IDictionary<string, string> Details { get; }
}