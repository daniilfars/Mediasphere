using Application.Interfaces;
using Domain;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class CommentDbContext : DbContext, ICommentDbContext
{
    public DbSet<Comment> Comments { get; set; }

    public CommentDbContext() { }
    public CommentDbContext(DbContextOptions<CommentDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        //builder.ApplyConfiguration(new CommentConfiguration());
        builder.AddTransactionalOutboxEntities();
    }
}