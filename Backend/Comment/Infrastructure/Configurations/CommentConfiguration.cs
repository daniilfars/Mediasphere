using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.Property(p => p.AuthorId).IsRequired();
        builder.Property(p => p.PostId).IsRequired();
        builder.Property(p => p.UserName).IsRequired();
        builder.Property(p => p.Content).HasMaxLength(16384).IsRequired();
        builder.Property(p => p.Likes).IsRequired();
    }
}
