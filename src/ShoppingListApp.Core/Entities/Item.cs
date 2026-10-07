namespace ShoppingListApp.Core.Entities;

public class Item
{
    public int Id { get; set; }
    public int ShoppingListId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsBought { get; set; }
    public int SortOrder { get; set; }
    public ShoppingList? ShoppingList { get; set; }
}