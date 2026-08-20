using MediatR;
using Shared.Domain;

namespace Application.Commands.UploadPostImage;

public sealed record UploadPostImageCommand(Guid Id, Guid AuthorId, Stream FileStream, string ContentType) : IRequest<Result<UploadPostImageResponse>>;