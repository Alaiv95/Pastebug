using Microsoft.EntityFrameworkCore;
using Pastebug.Domain.Entities;

namespace Pastebug.DAL;

public class PastebugDbContext : DbContext, IPastebugDbContext
{
    public DbSet<Paste> Pastes { get; set; }
    public DbSet<Exposure> Exposure { get; set; }
    public DbSet<PasteExposure> PasteExposure { get; set; }
    public DbSet<User> Users { get; set; }

    public PastebugDbContext(DbContextOptions<PastebugDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(x => x.Id);
        modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();

        modelBuilder.Entity<Paste>().HasKey(x => x.Hash);
        modelBuilder.Entity<Paste>().HasIndex(x => x.Hash).IsUnique();
        modelBuilder.Entity<Paste>().Property(x => x.Text).IsRequired();

        modelBuilder.Entity<PasteExposure>().HasKey(x => x.Id);
        modelBuilder.Entity<PasteExposure>().HasIndex(x => x.Id).IsUnique();

        modelBuilder.Entity<Paste>()
            .HasOne(p => p.PasteExposure)
            .WithOne(e => e.Paste)
            .HasForeignKey<PasteExposure>();

        modelBuilder.Entity<Exposure>().HasKey(x => x.Id);
        modelBuilder.Entity<Exposure>().HasIndex(x => x.Id).IsUnique();

        modelBuilder.Entity<Exposure>()
            .HasData(
                new Exposure { Id = 1, Type = "Public"},
                new Exposure { Id = 2, Type = "Private"}
            );

        modelBuilder.Entity<User>()
            .HasData(
                new User {Email = "admin@mail.ru"}
            );

        base.OnModelCreating(modelBuilder);
    }
}
