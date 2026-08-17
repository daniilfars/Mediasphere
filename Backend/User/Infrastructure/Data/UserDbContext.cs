using Application.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class UserDbContext : DbContext, IUserDbContext
{
    public DbSet<User> Users { get; set; }

    public UserDbContext() { }
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        //builder.ApplyConfiguration(new UserConfiguration());
    }
}
