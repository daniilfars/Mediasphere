using Application.Interfaces;
using Domain;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class LikeDbContext : DbContext, ILikeDbContext
{
    public DbSet<Like> Likes { get; set; }

    public LikeDbContext() { }
    public LikeDbContext(DbContextOptions<LikeDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasPostgresEnum<LikeTargetType>();
        builder.ApplyConfiguration(new LikeConfiguration());
    }
}
