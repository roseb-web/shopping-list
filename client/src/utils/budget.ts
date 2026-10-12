import type { Budget } from '../types'
import { sumPrices, toPence } from './money'

// Same rules as BudgetCalculator.cs, so the screen can update straight away
// without waiting for the server. Every item counts, bought or not.
export function getBudget(prices: number[], spendingLimit: number | null): Budget {
  const total = sumPrices(prices)

  if (spendingLimit === null) {
    return { total, spendingLimit: null, remaining: null, isOverBudget: false }
  }

  const remaining = (toPence(spendingLimit) - toPence(total)) / 100

  return { total, spendingLimit, remaining, isOverBudget: remaining < 0 }
}
