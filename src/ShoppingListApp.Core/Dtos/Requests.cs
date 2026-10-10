
namespace ShoppingListApp.Core.Dtos;

public record AddItemRequest(string Name, decimal Price);

public record SetBoughtRequest(int ItemId, bool IsBought);

public record ReorderItemsRequest(List<int>? ItemIds);

public record SetSpendingLimitRequest(decimal? SpendingLimit);