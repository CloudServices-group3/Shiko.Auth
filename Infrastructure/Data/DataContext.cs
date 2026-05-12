using Domain.Entities;
using Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class DataContext : IdentityDbContext<AppUser, IdentityRole, string> // Using Identity. The "string" indicates which data type ID is.
{
    public DataContext(DbContextOptions<DataContext> options) // DbContextOptions to get connection string, EF Core configuration and what database to use.
        : base(options)
    { 
    
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); //  Creates all the Identity tabels in the database.

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly); // Using the cofigurations for EF Core.

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.UserId)
                    .HasMaxLength(450)
                    .IsRequired();

            entity.Property(x => x.TokenHash)
                    .HasMaxLength(200)
                    .IsRequired();

            entity.Property(x => x.CreatedByIp)
                    .HasMaxLength(64);

            entity.Property(x => x.RevokedByIp)
                    .HasMaxLength(64);

            entity.HasIndex(x => x.TokenHash)
                    .IsUnique();

            entity.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
}
