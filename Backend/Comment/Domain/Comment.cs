using Shared.Domain;

namespace Domain;

public class Comment
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid PostId { get; private set; }
    public string Content { get; private set; }

    private Comment(Guid id, Guid userId, Guid postId, string content)
    {
        Id = id;
        UserId = userId;
        PostId = postId;
        Content = content;
    }

    public static Result<Comment> Create(Guid userId, Guid postId, string content)
    {
        if (userId == Guid.Empty || postId == Guid.Empty)
            return Result<Comment>.Failure("ID пользователя или поста не могут быть пустыми");

        if (string.IsNullOrWhiteSpace(content))
            return Result<Comment>.Failure("Контент комментария не может быть пустым");

        return Result<Comment>.Success(new Comment(Guid.NewGuid(), userId, postId, content));
    }

    public Result UpdateContent(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return Result.Failure("Контент комментария не может быть пустым");

        Content = content;
        return Result.Success();
    }
}