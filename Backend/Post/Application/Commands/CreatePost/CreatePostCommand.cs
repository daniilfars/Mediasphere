using MediatR;
using Shared.Domain;

namespace Application.Commands.CreatePost;

public sealed record CreatePostCommand(Guid AuthorId, string UserName, string Content, Stream FileStream, string ContentType) : IRequest<Result<CreatePostResponse>>;
