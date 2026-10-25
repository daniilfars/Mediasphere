using MediatR;
using Shared.Domain;
using System.ComponentModel.DataAnnotations;

namespace Application.Queries.GetPostComments;

public sealed record GetPostCommentsQuery(Guid PostId,
    [Range(1, int.MaxValue)] int Page = 1,
    [Range(1, 50)] int PageSize = 10)
    : IRequest<Result<GetPostCommentsResponse>>;