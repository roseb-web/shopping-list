using ShoppingListApp.Core.Results;
using ShoppingListApp.Core.Dtos;

namespace ShoppingListApp.Core.Services;

public interface IShoppingListService
{
    Task<ServiceResult<ShoppingListDto>> GetShoppingListAsync(int listId, CancellationToken ct = default);

    Task<ServiceResult<ItemDto>> AddItemAsync(int listId, AddItemRequest request, CancellationToken ct = default);

    Task<ServiceResult> RemoveItemAsync(int listId, int itemId, CancellationToken ct = default);

    Task<ServiceResult<ItemDto>> SetItemBoughtAsync(int listId, int itemId, bool isBought, CancellationToken ct = default);

    Task<ServiceResult> ReorderItemsAsync(int listId, List<int> itemIds, CancellationToken ct = default);

    Task<ServiceResult<BudgetSummary>> SetSpendingLimitAsync(int listId, decimal? spendingLimit, CancellationToken ct = default);

}