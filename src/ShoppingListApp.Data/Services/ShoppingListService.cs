using Microsoft.EntityFrameworkCore;
using ShoppingListApp.Core.Budget;
using ShoppingListApp.Core.Dtos;
using ShoppingListApp.Core.Entities;
using ShoppingListApp.Core.Results;
using ShoppingListApp.Core.Services;
using ShoppingListApp.Core.Validation;

namespace ShoppingListApp.Data.Services;

public class ShoppingListService : IShoppingListService
{
    private readonly ShoppingListDbContext _dbContext;

    public ShoppingListService(ShoppingListDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<ServiceResult<ShoppingListDto>> GetShoppingListAsync(int listId, CancellationToken ct = default)
    {
        var shoppingList = await _dbContext.ShoppingLists
            .Include(sl => sl.Items.OrderBy(i => i.SortOrder))
            .FirstOrDefaultAsync(sl => sl.Id == listId, ct);

        if (shoppingList == null)
        {
            return ServiceResult<ShoppingListDto>.Fail(ServiceError.NotFound, "Shopping list not found.");
        }

        // Calculate budget summary
        var summary = BudgetCalculator.GetSummary(shoppingList.Items, shoppingList.SpendingLimit);

        // Map to DTO
        var dto = new ShoppingListDto(
            shoppingList.Name,
            shoppingList.Items.Select(i => new ItemDto(i.Id, i.Name, i.Price, i.IsBought)).ToList(),
            shoppingList.SpendingLimit,
            summary.Total,
            summary.Remaining,
            summary.IsOverBudget
        );

        return ServiceResult<ShoppingListDto>.Ok(dto);
    }


    public async Task<ServiceResult<ItemDto>> AddItemAsync(int listId, AddItemRequest request, CancellationToken ct = default)
    {
        var shoppingList = await _dbContext.ShoppingLists
            .Include(sl => sl.Items)
            .FirstOrDefaultAsync(sl => sl.Id == listId, ct);

        if (shoppingList == null)
        {
            return ServiceResult<ItemDto>.Fail(ServiceError.NotFound, "Shopping list not found.");
        }

        var nameValidation = ItemRules.ValidateName(request.Name);
        if (!string.IsNullOrEmpty(nameValidation))
        {
            return ServiceResult<ItemDto>.Fail(ServiceError.ValidationError, nameValidation);
        }

        var priceValidation = ItemRules.ValidatePrice(request.Price);
        if (!string.IsNullOrEmpty(priceValidation))
        {
            return ServiceResult<ItemDto>.Fail(ServiceError.ValidationError, priceValidation);
        }

        var name = request.Name.Trim();

        // Check for duplicate item names
        if (shoppingList.Items.Any(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            return ServiceResult<ItemDto>.Fail(ServiceError.ValidationError, "An item with the same name already exists in the shopping list.");
        }

        // Set the sort order for the new item
        var maxSortOrder = shoppingList.Items.Any() ? shoppingList.Items.Max(i => i.SortOrder) : 0;

        // Create and add the new item
        var newItem = new Item
        {
            Name = name,
            Price = request.Price,
            SortOrder = maxSortOrder + 1,
            IsBought = false
        };

        shoppingList.Items.Add(newItem);

        await _dbContext.SaveChangesAsync(ct);

        return ServiceResult<ItemDto>.Ok(new ItemDto(newItem.Id, newItem.Name, newItem.Price, newItem.IsBought));
    }

    public async Task<ServiceResult> RemoveItemAsync(int listId, int itemId, CancellationToken ct = default)
    {
        var shoppingList = await _dbContext.ShoppingLists
            .Include(sl => sl.Items)
            .FirstOrDefaultAsync(sl => sl.Id == listId, ct);

        if (shoppingList == null)
        {
            return ServiceResult.Fail(ServiceError.NotFound, "Shopping list not found.");
        }

        var item = shoppingList.Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
        {
            return ServiceResult.Fail(ServiceError.NotFound, "Item not found in the shopping list.");
        }

        shoppingList.Items.Remove(item);

        await _dbContext.SaveChangesAsync(ct);

        return ServiceResult.Ok();
    }

    public async Task<ServiceResult<ItemDto>> SetItemBoughtAsync(int listId, int itemId, bool isBought, CancellationToken ct = default)
    {
        var shoppingList = await _dbContext.ShoppingLists
            .Include(sl => sl.Items)
            .FirstOrDefaultAsync(sl => sl.Id == listId, ct);

        if (shoppingList == null)
        {
            return ServiceResult<ItemDto>.Fail(ServiceError.NotFound, "Shopping list not found.");
        }

        var item = shoppingList.Items.FirstOrDefault(i => i.Id == itemId);

        if (item == null)
        {
            return ServiceResult<ItemDto>.Fail(ServiceError.NotFound, "Item not found in the shopping list.");
        }

        item.IsBought = isBought;

        await _dbContext.SaveChangesAsync(ct);

        return ServiceResult<ItemDto>.Ok(new ItemDto(item.Id, item.Name, item.Price, item.IsBought));
    }

    public async Task<ServiceResult> ReorderItemsAsync(int listId, List<int> itemIds, CancellationToken ct = default)
    {
        var shoppingList = await _dbContext.ShoppingLists
            .Include(sl => sl.Items)
            .FirstOrDefaultAsync(sl => sl.Id == listId, ct);

        if (shoppingList == null)
        {
            return ServiceResult.Fail(ServiceError.NotFound, "Shopping list not found.");
        }

        // Validate that all provided item IDs exist in the shopping list
        var existingItemIds = shoppingList.Items.Select(i => i.Id).ToHashSet();

        if(existingItemIds.Count != itemIds.Count)
        {
            return ServiceResult.Fail(ServiceError.ValidationError, "The number of provided item IDs does not match the number of items in the shopping list.");
        }

        if (!itemIds.All(id => existingItemIds.Contains(id)))
        {
            return ServiceResult.Fail(ServiceError.ValidationError, "One or more item IDs do not exist in the shopping list.");
        }

        // Update sort order based on the new order
        for (int i = 0; i < itemIds.Count; i++)
        {
            var item = shoppingList.Items.First(it => it.Id == itemIds[i]);
            item.SortOrder = i + 1;
        }

        await _dbContext.SaveChangesAsync(ct);

        return ServiceResult.Ok();
    }

    public async Task<ServiceResult<BudgetSummary>> SetSpendingLimitAsync(int listId, decimal? spendingLimit, CancellationToken ct = default)
    {
        var shoppingList = await _dbContext.ShoppingLists
            .Include(sl => sl.Items)
            .FirstOrDefaultAsync(sl => sl.Id == listId, ct);

        if (shoppingList == null)
        {
            return ServiceResult<BudgetSummary>.Fail(ServiceError.NotFound, "Shopping list not found.");
        }

        var limitValidation = ItemRules.ValidateSpendingLimit(spendingLimit);
        if (!string.IsNullOrEmpty(limitValidation))
        {
            return ServiceResult<BudgetSummary>.Fail(ServiceError.ValidationError, limitValidation);
        }

        shoppingList.SpendingLimit = spendingLimit;

        await _dbContext.SaveChangesAsync(ct);

        var summary = BudgetCalculator.GetSummary(shoppingList.Items, shoppingList.SpendingLimit);


        return ServiceResult<BudgetSummary>.Ok(summary);
    }
}