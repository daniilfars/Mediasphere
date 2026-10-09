using Domain;

namespace WebAPI.Models;

public sealed record DeleteLikeByContendDto(LikeTargetType TargetType, Guid ContentId);
