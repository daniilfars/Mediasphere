using Application.Interfaces;
using Domain;
using Grpc.Core;
using LikeGrpc;
using Microsoft.EntityFrameworkCore;

namespace Application.Grpc;

public class LikeGrpcService : LikeService.LikeServiceBase
{
    private readonly ILikeDbContext _db;

    public LikeGrpcService(ILikeDbContext db)
    {
        _db = db;
    }

    public override async Task<CheckLikesResponse> CheckLikes(CheckLikesRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.UserId, out var userId))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Невалидный Guid у UserId"));

        if (request.TargetType == TargetType.Unspecified)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "TargetType не указан"));

        if (request.ContentIds == null || !request.ContentIds.Any())
            return new CheckLikesResponse();

        var contentGuids = request.ContentIds
            .Select(id => Guid.TryParse(id, out var parsedGuid) ? parsedGuid : Guid.Empty)
            .Where(guid => guid != Guid.Empty)
            .ToList();

        LikeTargetType targetType = request.TargetType == TargetType.Post ? LikeTargetType.Post : LikeTargetType.Comment;

        var likedIds = await _db.Likes.AsNoTracking()
            .Where(l => l.UserId == userId && l.TargetType == targetType && contentGuids.Contains(l.ContentId))
            .Select(l => l.ContentId.ToString())
            .ToListAsync();

        var likedSet = likedIds.ToHashSet();

        var response = new CheckLikesResponse();

        foreach (var originalId in request.ContentIds)
        {
            response.Results.Add(new LikedPostStatus
            {
                ContentId = originalId,
                IsLiked = likedSet.Contains(originalId)
            });
        }

        return response;
    }
}