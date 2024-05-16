using Microsoft.EntityFrameworkCore;
using UserBlock.Domain;

namespace UserBlock.Infrastructure;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase(databaseName: "UserDb");
    }

    public DbSet<User> Users { get; init; }
}
