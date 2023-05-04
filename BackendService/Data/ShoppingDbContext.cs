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
    public DbSet<Location> Locations { get; set; }
    public DbSet<LocationProduct> LocationProducts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(b => b.Id);
        modelBuilder.Entity<User>()
            .HasIndex(b => b.Email).IsUnique();
        modelBuilder.Entity<LocationProduct>()
            .HasKey(lp => new { lp.LocationId, lp.ProductId }); 

        modelBuilder.Entity<Product>().Property(p => p.ImageURLs)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<string>>(v));
    }
}