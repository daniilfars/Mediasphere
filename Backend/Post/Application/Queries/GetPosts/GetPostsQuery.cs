using MediatR;
using Shared.Domain;

namespace Application.Queries.GetPosts;

public sealed record GetPostsQuery(int Page = 1, int PageSize = 10) : IRequest<Result<GetPostsResponse>>;