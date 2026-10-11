
namespace ShoppingListApp.Core.Dtos;

public record AddItemRequest(string Name, decimal Price);

public record SetBoughtRequest(bool IsBought);

public record ReorderItemsRequest(List<int>? ItemIds);

public record SetSpendingLimitRequest(decimal? SpendingLimit);