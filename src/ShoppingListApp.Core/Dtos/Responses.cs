
namespace ShoppingListApp.Core.Dtos;

  public record BudgetSummary(decimal Total, decimal? SpendingLimit, decimal? Remaining, bool IsOverBudget);

  public record ItemDto(int Id, string Name, decimal Price, bool IsBought);

  public record ShoppingListDto(string Name, List<ItemDto> Items, decimal? SpendingLimit, BudgetSummary? BudgetSummary);