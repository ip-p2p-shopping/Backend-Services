using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using BackendService.Data.Models;

namespace BackendService.Data;

public class ShoppingDbContext : DbContext
{
    public ShoppingDbContext(DbContextOptions<ShoppingDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(b => b.Id);
        modelBuilder.Entity<User>()
            .HasIndex(b => b.Email).IsUnique();
    }
}