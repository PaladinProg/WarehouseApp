using Microsoft.EntityFrameworkCore;
using WarehouseAppFull.Models;

namespace WarehouseAppFull.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<Receipt> Receipts => Set<Receipt>();
    public DbSet<ReceiptItem> ReceiptItems => Set<ReceiptItem>();
    public DbSet<StockBalance> StockBalances => Set<StockBalance>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Unit>()
            .HasIndex(u => u.Name)
            .IsUnique();

        modelBuilder.Entity<Resource>()
            .HasIndex(r => r.Name)
            .IsUnique();

        modelBuilder.Entity<Receipt>()
            .HasIndex(r => r.Number)
            .IsUnique();

        modelBuilder.Entity<ReceiptItem>()
            .HasOne(i => i.Resource)
            .WithMany()
            .HasForeignKey(i => i.ResourceId);

        modelBuilder.Entity<ReceiptItem>()
            .HasOne(i => i.Unit)
            .WithMany()
            .HasForeignKey(i => i.UnitId);

        modelBuilder.Entity<ReceiptItem>()
            .HasOne(i => i.Receipt)
            .WithMany(r => r.Items)
            .HasForeignKey(i => i.ReceiptId);

        modelBuilder.Entity<StockBalance>()
            .HasIndex(b => new { b.ResourceId, b.UnitId })
            .IsUnique();
    }
}