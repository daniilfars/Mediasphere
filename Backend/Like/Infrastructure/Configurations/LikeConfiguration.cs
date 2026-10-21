using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class LikeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.Property(l => l.UserId).IsRequired();
        builder.Property(l => l.TargetType).IsRequired();

        builder.HasIndex(l => new { l.UserId, l.TargetType, l.ContentId }).IsUnique();
        builder.HasIndex(l => new { l.TargetType, l.ContentId });
    }
}
