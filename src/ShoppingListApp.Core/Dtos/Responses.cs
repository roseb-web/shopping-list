
namespace ShoppingListApp.Core.Dtos;

  public record BudgetSummary(decimal Total, decimal? SpendingLimit, decimal? Remaining, bool IsOverBudget);