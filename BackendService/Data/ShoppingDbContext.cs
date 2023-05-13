using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using BackendService.Data.Models;
using Newtonsoft.Json;

namespace BackendService.Data;

public class ShoppingDbContext : DbContext
{
    public ShoppingDbContext(DbContextOptions<ShoppingDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<ShoppingInstance> ShoppingInstances { get; set; }
    public DbSet<FavouriteProduct> FavouriteProducts { get; set; }
    public DbSet<FavouriteStore> FavouriteStores { get; set; }
    public DbSet<GhostLocation> GhostLocations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(b => b.Id);
        modelBuilder.Entity<User>()
            .HasIndex(b => b.Email).IsUnique();
        modelBuilder.Entity<User>()
            .HasIndex(b => b.StoreId).IsUnique();

        modelBuilder.Entity<Product>().HasIndex(p => p.StoreId);
        modelBuilder.Entity<Product>().Property(p => p.ImageURLs)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<string>>(v));

        modelBuilder.Entity<ShoppingInstance>().HasIndex(x => x.ProductId);
        modelBuilder.Entity<ShoppingInstance>().HasIndex(x => x.UserId);
        modelBuilder.Entity<ShoppingInstance>().HasIndex(x => x.Bought);

        modelBuilder.Entity<GhostLocation>().HasIndex(x => x.ProductId);
    }
}