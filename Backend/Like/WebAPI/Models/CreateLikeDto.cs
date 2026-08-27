using Domain;

namespace WebAPI.Models;

public sealed record CreateLikeDto(LikeTargetType TargetType, Guid ContentId);
