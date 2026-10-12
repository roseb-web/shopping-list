import { describe, expect, it } from 'vitest'
import { getBudget } from './budget'

describe('getBudget', () => {
  it('is 0 for an empty list with no limit', () => {
    expect(getBudget([], null)).toEqual({ total: 0, spendingLimit: null, remaining: null, isOverBudget: false })
  })

  it('has no remaining amount when there is no limit', () => {
    const budget = getBudget([1.2, 0.8], null)

    expect(budget.total).toBe(2)
    expect(budget.remaining).toBeNull()
    expect(budget.isOverBudget).toBe(false)
  })

  it('shows money left when under the limit', () => {
    const budget = getBudget([1.2, 0.8], 5)

    expect(budget.remaining).toBe(3)
    expect(budget.isOverBudget).toBe(false)
  })

  it('is not over budget when exactly on the limit', () => {
    const budget = getBudget([0.1, 0.2], 0.3)

    expect(budget.remaining).toBe(0)
    expect(budget.isOverBudget).toBe(false)
  })

  it('is flagged when over the limit', () => {
    const budget = getBudget([1.2, 0.8], 1)

    expect(budget.remaining).toBe(-1)
    expect(budget.isOverBudget).toBe(true)
  })
})
