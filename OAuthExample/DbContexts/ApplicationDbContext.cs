using Microsoft.EntityFrameworkCore;
using OAuthExample.Entites;

namespace OAuthExample.DbContexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options)
    : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}
