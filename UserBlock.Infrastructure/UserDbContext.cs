using Microsoft.EntityFrameworkCore;
using UserBlock.Contracts;
using UserBlock.Domain;

namespace UserBlock.Infrastructure;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User(Guid.NewGuid(), "user1", BCrypt.Net.BCrypt.HashPassword("password1"), "user1@gmail.com",
                []),
            new User(Guid.NewGuid(), "user2", BCrypt.Net.BCrypt.HashPassword("password2"), "user2@gmail.com",
                []));
    }

    public DbSet<User> Users { get; set; }
}