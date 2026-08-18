using Application.Interfaces;
using Domain;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class PostDbContext : DbContext, IPostDbContext
{
    public DbSet<Post> Posts { get; set; }

    public PostDbContext() { }
    public PostDbContext(DbContextOptions<PostDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new PostConfiguration());
    }
}
