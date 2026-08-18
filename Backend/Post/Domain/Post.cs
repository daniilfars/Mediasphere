using Shared.Domain;

namespace Domain;

public class Post
{
    public Guid Id { get; private set; }
    public Guid AuthorId { get; private set; }
    public string UserName { get; private set; }
    public string Content { get; private set; }
    public long Likes { get; private set; }
    public string? ImageUrl { get; private set; }

    private Post(Guid authorId, string userName, string content, string? imageUrl)
    {
        Id = Guid.CreateVersion7();
        //Console.WriteLine(Id);
        AuthorId = authorId;
        UserName = userName;
        Content = content;
        Likes = 0;
        ImageUrl = imageUrl;
    }

    public static Result<Post> Create(Guid authorId, string userName, string content, string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return Result<Post>.Failure("Никнейм не может быть пустым");

        if (string.IsNullOrWhiteSpace(content))
            return Result<Post>.Failure("Контент поста не может быть пустым");

        return Result<Post>.Success(new Post(authorId, userName, content, imageUrl));
    }

    public void UpdateImageUrl(string imageUrl)
    {
        ImageUrl = imageUrl;
    }
}