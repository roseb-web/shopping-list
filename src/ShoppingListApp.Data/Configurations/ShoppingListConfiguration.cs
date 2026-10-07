using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingListApp.Core.Entities;

namespace ShoppingListApp.Data.Configurations;

public class ShoppingListConfiguration : IEntityTypeConfiguration<ShoppingList>
{
    public const int DefaultListId = 1;
    
    public void Configure(EntityTypeBuilder<ShoppingList> builder)
    {
        builder.Property(sl => sl.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sl => sl.SpendingLimit)
            .HasPrecision(10, 2);

        builder.HasMany(sl => sl.Items)
            .WithOne(i => i.ShoppingList)
            .HasForeignKey(i => i.ShoppingListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(new ShoppingList
        {
            Id = DefaultListId,
            Name = "My shopping list",
            SpendingLimit = null
        });
    }
}