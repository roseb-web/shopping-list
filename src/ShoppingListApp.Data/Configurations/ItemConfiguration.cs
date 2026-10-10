using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingListApp.Core.Entities;
using ShoppingListApp.Core.Validation;

namespace Data.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(ItemRules.NameMaxLength)
            .UseCollation("NOCASE"); // Milk and milk are considered the same item

        builder.Property(i => i.Price)
            .HasPrecision(10, 2);

        builder.Property(i => i.SortOrder);

        builder.Property(i => i.IsBought);

        builder.HasIndex(i => new { i.ShoppingListId, i.SortOrder });
            
        // No duplicate items on the list
        builder.HasIndex(i => new { i.ShoppingListId, i.Name }).IsUnique();
    }
}