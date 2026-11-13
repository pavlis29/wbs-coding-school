using ChallengeTrackerApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChallengeTrackerApi.Data;

public class ApplicationDbContext(DbContextOptions options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Challenge> Challenges => Set<Challenge>();
    public DbSet<Membership> Memberships => Set<Membership>();
    public DbSet<ProgressEntry> ProgressEntries => Set<ProgressEntry>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        base.OnModelCreating(model);
        model.Entity<AppMetadata>();
        model.Entity<User>().Property(x => x.DisplayName).HasMaxLength(60).IsRequired();
        model.Entity<Challenge>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(90).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.Category).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Unit).HasMaxLength(24).IsRequired();
            entity.Property(x => x.Target).HasPrecision(38, 28);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(16);
            entity.Property(x => x.Visibility).HasConversion<string>().HasMaxLength(16);
            entity.Property(x => x.CreatedAt).HasConversion(x => x.UtcTicks, x => new DateTimeOffset(x, TimeSpan.Zero));
            entity.Property(x => x.Version).IsConcurrencyToken();
            entity.HasOne(x => x.Owner).WithMany(x => x.OwnedChallenges)
                .HasForeignKey(x => x.OwnerId).OnDelete(DeleteBehavior.Restrict);
        });
        model.Entity<Membership>(entity =>
        {
            entity.HasIndex(x => new { x.UserId, x.ChallengeId }).IsUnique();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(16);
            entity.HasOne(x => x.User).WithMany(x => x.Memberships)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Challenge).WithMany(x => x.Memberships)
                .HasForeignKey(x => x.ChallengeId).OnDelete(DeleteBehavior.Cascade);
        });
        model.Entity<ProgressEntry>(entity =>
        {
            entity.HasIndex(x => new { x.UserId, x.ChallengeId, x.Day }).IsUnique();
            entity.Property(x => x.Amount).HasPrecision(38, 28);
            entity.Property(x => x.Note).HasMaxLength(250);
            entity.Property(x => x.LoggedAt).HasConversion(x => x.UtcTicks, x => new DateTimeOffset(x, TimeSpan.Zero));
            entity.Property(x => x.CreatedAt).HasConversion(x => x.UtcTicks, x => new DateTimeOffset(x, TimeSpan.Zero));
            entity.Property(x => x.Version).IsConcurrencyToken();
            entity.HasOne(x => x.User).WithMany(x => x.ProgressEntries)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Challenge).WithMany(x => x.ProgressEntries)
                .HasForeignKey(x => x.ChallengeId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}

// Each provider owns its migrations and snapshot. The services use the base context.
public class SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) : ApplicationDbContext(options);
