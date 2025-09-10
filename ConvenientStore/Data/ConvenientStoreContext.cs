using Microsoft.EntityFrameworkCore;
using ConvenientStore.Models;

namespace ConvenientStore.Data;

public class ConvenientStoreContext : DbContext
{
    public ConvenientStoreContext(DbContextOptions<ConvenientStoreContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.HasOne(e => e.Category)
                  .WithMany(e => e.Products)
                  .HasForeignKey(e => e.CategoryId);
        });

        // Configure Category
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        // Configure Sale
        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalAmount).HasPrecision(10, 2);
        });

        // Configure SaleItem
        modelBuilder.Entity<SaleItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasPrecision(10, 2);
            entity.HasOne(e => e.Sale)
                  .WithMany(e => e.SaleItems)
                  .HasForeignKey(e => e.SaleId);
            entity.HasOne(e => e.Product)
                  .WithMany()
                  .HasForeignKey(e => e.ProductId);
        });

        // Seed initial data
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Beverages", Description = "Drinks and beverages" },
            new Category { Id = 2, Name = "Snacks", Description = "Snack foods and confectionery" },
            new Category { Id = 3, Name = "Dairy", Description = "Milk and dairy products" },
            new Category { Id = 4, Name = "Personal Care", Description = "Personal hygiene and care products" }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Coca-Cola", Description = "Refreshing cola drink", Price = 1.50m, StockQuantity = 100, CategoryId = 1, Barcode = "123456789001" },
            new Product { Id = 2, Name = "Pepsi", Description = "Cola beverage", Price = 1.45m, StockQuantity = 80, CategoryId = 1, Barcode = "123456789002" },
            new Product { Id = 3, Name = "Chips", Description = "Potato chips", Price = 2.99m, StockQuantity = 50, CategoryId = 2, Barcode = "123456789003" },
            new Product { Id = 4, Name = "Chocolate Bar", Description = "Milk chocolate", Price = 1.99m, StockQuantity = 75, CategoryId = 2, Barcode = "123456789004" },
            new Product { Id = 5, Name = "Milk", Description = "Fresh whole milk", Price = 3.49m, StockQuantity = 30, CategoryId = 3, Barcode = "123456789005" },
            new Product { Id = 6, Name = "Toothpaste", Description = "Fluoride toothpaste", Price = 4.99m, StockQuantity = 25, CategoryId = 4, Barcode = "123456789006" }
        );
    }
}