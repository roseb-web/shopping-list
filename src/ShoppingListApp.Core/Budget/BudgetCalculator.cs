using ShoppingListApp.Core.Dtos;
using ShoppingListApp.Core.Entities;

namespace ShoppingListApp.Core.Budget;

public static class BudgetCalculator
{
    public static decimal GetTotalSpent(IEnumerable<Item> items)
    {
        return items.Sum(i => i.Price);
    }

    public static BudgetSummary GetSummary(IEnumerable<Item> items, decimal? spendingLimit)
    {
        var total = GetTotalSpent(items);
        var remaining = spendingLimit.HasValue ? spendingLimit.Value - total : (decimal?)null;
        var isOverBudget = remaining.HasValue && remaining.Value < 0;

        return new BudgetSummary(total, spendingLimit, remaining, isOverBudget);
    }
}