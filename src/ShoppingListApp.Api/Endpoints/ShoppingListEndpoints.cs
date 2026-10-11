using ShoppingListApp.Core.Dtos;
using ShoppingListApp.Core.Services;
using ShoppingListApp.Data.Configurations;

namespace ShoppingListApp.Api.Endpoints;

public static class ShoppingListEndpoints
{
    // The app has a single list, seeded by ShoppingListConfiguration
    // The list ID is hardcoded here, but could be made dynamic in the future if we add multiple lists
    private const int ListId = ShoppingListConfiguration.DefaultListId;

    // Map some endpoints to the shopping list service methods, to then reference in our FE
    public static void MapShoppingListEndpoints(this WebApplication app)
    {
        var list = app.MapGroup("/api/list");

        // Get: Read
        list.MapGet("/", GetShoppingList);
        // Post: Create
        list.MapPost("/items", AddItem);
        // Delete: Delete (duh)
        list.MapDelete("/items/{id:int}", RemoveItem);
        // Patch: Change one bit
        list.MapPatch("/items/{id:int}/bought", SetBoughtStatus);
        // Put: Change the whole thing
        list.MapPut("/items/order", ReorderItems);
        list.MapPut("/limit", SetSpendingLimit);
    }


    // Boring methods, one for each endpoint, that call the service and return the result
    private static async Task<IResult> GetShoppingList(IShoppingListService shoppingListService, CancellationToken ct)
    {
        var result = await shoppingListService.GetShoppingListAsync(ListId, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> AddItem(IShoppingListService shoppingListService, AddItemRequest request, CancellationToken ct)
    {
        var result = await shoppingListService.AddItemAsync(ListId, request, ct);
        return result.ToHttpResult();
    }


    private static async Task<IResult> RemoveItem(IShoppingListService shoppingListService, int id, CancellationToken ct)
    {
        var result = await shoppingListService.RemoveItemAsync(ListId, id, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> SetBoughtStatus(IShoppingListService shoppingListService, int id, SetBoughtRequest request, CancellationToken ct)
    {
        var result = await shoppingListService.SetItemBoughtAsync(ListId, id, request.IsBought, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> ReorderItems(IShoppingListService shoppingListService, ReorderItemsRequest request, CancellationToken ct)
    {
        var result = await shoppingListService.ReorderItemsAsync(ListId, request.ItemIds ?? [], ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> SetSpendingLimit(IShoppingListService shoppingListService, SetSpendingLimitRequest request, CancellationToken ct)
    {
        var result = await shoppingListService.SetSpendingLimitAsync(ListId, request.SpendingLimit, ct);
        return result.ToHttpResult();
    }

}
