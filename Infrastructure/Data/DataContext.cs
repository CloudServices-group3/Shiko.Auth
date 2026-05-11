using Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class DataContext : IdentityDbContext<AppUser> // Using Identity.
{
    public DataContext(DbContextOptions<DataContext> options) // DbContextOptions to get connection string, EF Core configuration and what database to use.
        : base(options)
    { 
    
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); //  Creates all the Identity tabels in the database.

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly); // Using the cofigurations for EF Core.
    }
}
