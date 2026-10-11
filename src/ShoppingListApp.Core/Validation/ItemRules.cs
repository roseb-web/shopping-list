namespace ShoppingListApp.Core.Validation;

public static class ItemRules
{
    public const int NameMaxLength = 100;
    public const decimal MaxPrice = 10_000m;
    public const decimal MaxSpendingLimit = 10_000m;

    public static string ValidateName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && name.Length <= NameMaxLength ? "" : $"Name must be between 1 and {NameMaxLength} characters.";
    }

    public static string ValidatePrice(decimal price)
    {
        return price >= 0 && price <= MaxPrice ? "" : $"Price must be between 0 and {MaxPrice}.";
    }

    public static string ValidateSpendingLimit(decimal? spendingLimit)
    {
        // No limit (null) is allowed, so the user can remove their spending limit
        return spendingLimit is null || (spendingLimit >= 0 && spendingLimit <= MaxSpendingLimit) ? "" : $"Spending limit must be between 0 and {MaxSpendingLimit}.";
    }
}