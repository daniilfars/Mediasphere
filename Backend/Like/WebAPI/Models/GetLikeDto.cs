using Domain;

namespace WebAPI.Models;

public sealed record GetLikeDto(LikeTargetType TargetType, Guid ContentId);