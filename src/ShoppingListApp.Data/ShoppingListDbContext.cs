using ShoppingListApp.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ShoppingListApp.Data;

public class ShoppingListDbContext : DbContext
{
    public ShoppingListDbContext(DbContextOptions<ShoppingListDbContext> options) : base(options)
    {
    }

    public DbSet<ShoppingList> ShoppingLists => Set<ShoppingList>();
    public DbSet<Item> Items => Set<Item>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShoppingListDbContext).Assembly);
    }
}