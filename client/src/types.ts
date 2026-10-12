// These match the C# DTOs in ShoppingListApp.Core/Dtos/Responses.cs.
// The API sends JSON with camelCase names, so ItemDto.IsBought arrives as isBought.

export interface Item {
  id: number
  name: string
  price: number
  isBought: boolean
}

export interface Budget {
  total: number
  spendingLimit: number | null
  remaining: number | null
  isOverBudget: boolean
}

export interface ShoppingList {
  name: string
  items: Item[]
  spendingLimit: number | null
  budgetSummary: Budget | null
}
