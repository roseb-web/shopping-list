using ShoppingListApp.Core.Dtos;
using ShoppingListApp.Core.Entities;
using ShoppingListApp.Data.Services;
using ShoppingListApp.Tests.Helpers;
using Xunit;

namespace ShoppingListApp.Tests.Data;

public class ShoppingListServiceTests
{

    [Fact]
    public async Task AddItem_SortOrderIsNext()
    {
        var testDatabase = new TestDatabase();

        using var arrangeDb = testDatabase.CreateDbContext();

        var shoppingList = new ShoppingList { Name = "Test List" , Items = new List<Item>
        {
            new Item { Name = "Item 1", Price = 1.00m, SortOrder = 0 },
            new Item { Name = "Item 2", Price = 2.00m, SortOrder = 1 }
        }};
        
        arrangeDb.Add(shoppingList);

        arrangeDb.SaveChanges();

        var service = new ShoppingListService(arrangeDb);

        var request = new AddItemRequest("Item 3", 3.00m);

        var result = await service.AddItemAsync(shoppingList.Id, request);

        Assert.True(result.IsSuccess);

        using var assertDb = testDatabase.CreateDbContext();

        var saved = assertDb.Items.First(i => i.Name == "Item 3");

        Assert.Equal(3, saved.SortOrder);
    } 

    // More tests can be added here to cover other methods of the ShoppingListService
}