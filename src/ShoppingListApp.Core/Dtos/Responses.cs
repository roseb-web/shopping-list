
namespace ShoppingListApp.Core.Dtos;

  public record BudgetSummary(decimal Total, decimal? SpendingLimit, decimal? Remaining, bool IsOverBudget);

  public record ItemDto(string Name, decimal Price);

  public record ShoppingListDto(string Name, List<ItemDto> Items, decimal? SpendingLimit, decimal TotalSpent, decimal? RemainingBudget, bool IsOverBudget);