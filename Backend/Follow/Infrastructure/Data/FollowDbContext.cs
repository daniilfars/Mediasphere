using Application.Interfaces;
using Domain;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class FollowDbContext : DbContext, IFollowDbContext
{
    public DbSet<Follow> Follows { get; set; }

    public FollowDbContext() { }
    public FollowDbContext(DbContextOptions<FollowDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new FollowConfiguration());
    }
}
