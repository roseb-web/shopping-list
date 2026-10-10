using Xunit;
using ShoppingListApp.Core.Budget;
using ShoppingListApp.Core.Entities;

namespace ShoppingListApp.Tests.Core;

public class BudgetCalculatorTests
{
    [Fact]
    public void GetTotalSpent_CalculatesCorrectly()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = "Milk", Price = 2.50m },
            new Item { Name = "Bread", Price = 1.75m },
            new Item { Name = "Eggs", Price = 3.00m }
        };

        // Act
        var totalSpent = BudgetCalculator.GetTotalSpent(items);

        // Assert
        Assert.Equal(7.25m, totalSpent);
    }

}