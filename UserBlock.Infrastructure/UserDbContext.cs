using Microsoft.EntityFrameworkCore;
using UserBlock.Contracts;
using UserBlock.Domain;

namespace UserBlock.Infrastructure;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase(databaseName: "UserDb");
    }

    public DbSet<User> Users { get; set; }
}