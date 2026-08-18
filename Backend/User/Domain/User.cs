using Shared.Domain;

namespace Domain;

public class User
{
    public Guid Id { get; private set; }
    public string UserName { get; private set; }

    private User(Guid id, string userName)
    {
        Id = id;
        UserName = userName;
    }

    public static Result<User> Create(Guid id, string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return Result<User>.Failure("Никнейм не может быть пустым");

        return Result<User>.Success(new User(id, userName));
    }
}