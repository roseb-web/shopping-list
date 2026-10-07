namespace ShoppingListApp.Core.Entities;

public class ShoppingList
{
    public int Id { get; set; }
    public string Name { get; set; } = "My Shopping List";
    public decimal? SpendingLimit { get; set; }
    public List<Item> Items { get; set; } = new();
    
    // Room to grow (story 10): add login by giving each list an owner,
    // e.g. public int UserId { get; set; }
}