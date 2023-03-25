using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace BackendService.Data;

public class ShoppingDbContext : DbContext
{
    public ShoppingDbContext(DbContextOptions<ShoppingDbContext> options) : base(options)
    {
    }
}