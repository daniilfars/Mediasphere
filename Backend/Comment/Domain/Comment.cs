using Shared.Domain;

namespace Domain;

public class Comment
{
    public Guid Id { get; private set; }
    public Guid AuthorId { get; private set; }
    public string UserName { get; private set; }
    public Guid PostId { get; private set; }
    public string Content { get; private set; }
    public long Likes { get; private set; }

    private Comment(Guid authorId, string userName, Guid postId, string content)
    {
        Id = Guid.CreateVersion7();
        AuthorId = authorId;
        UserName = userName;
        PostId = postId;
        Content = content;
        Likes = 0;
    }

    public static Result<Comment> Create(Guid authorId, string userName, Guid postId, string content)
    {
        if (authorId == Guid.Empty || postId == Guid.Empty)
            return Result<Comment>.Failure("ID пользователя или поста не могут быть пустыми");

        if (string.IsNullOrWhiteSpace(userName))
            return Result<Comment>.Failure("Имя пользователя не может быть пустым");

        if (string.IsNullOrWhiteSpace(content))
            return Result<Comment>.Failure("Контент комментария не может быть пустым");

        return Result<Comment>.Success(new Comment(authorId, userName, postId, content));
    }

    public Result UpdateContent(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return Result.Failure("Контент комментария не может быть пустым");

        Content = content;
        return Result.Success();
    }

    public void AddLike()
    {
        Likes++;
    }

    public void DeleteLike()
    {
        Likes--;
    }
}