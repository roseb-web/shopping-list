import { createPinia, setActivePinia } from 'pinia'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import * as api from '../api/shoppingListApi'
import { useShoppingListStore } from './shoppingList'

// Swap the real API calls for fakes, so these tests don't need the server running
vi.mock('../api/shoppingListApi', async importOriginal => ({
  ...(await importOriginal<typeof import('../api/shoppingListApi')>()),
  getList: vi.fn(),
  reorderItems: vi.fn(),
}))

function makeItem(id: number, name: string) {
  return { id, name, price: 1, isBought: false }
}

describe('shopping list store', () => {
  beforeEach(() => {
    // A fresh store for every test
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  it('moving an item down saves the new order', async () => {
    const store = useShoppingListStore()
    store.items = [makeItem(1, 'Milk'), makeItem(2, 'Bread'), makeItem(3, 'Eggs')]

    await store.moveItem(1, 1)

    expect(store.items.map(item => item.id)).toEqual([2, 1, 3])
    expect(api.reorderItems).toHaveBeenCalledWith([2, 1, 3])
  })

  it('moving the first item up does nothing', async () => {
    const store = useShoppingListStore()
    store.items = [makeItem(1, 'Milk'), makeItem(2, 'Bread')]

    await store.moveItem(1, -1)

    expect(store.items.map(item => item.id)).toEqual([1, 2])
    expect(api.reorderItems).not.toHaveBeenCalled()
  })

  it('reloads and shows the error when saving the order fails', async () => {
    const store = useShoppingListStore()
    store.items = [makeItem(1, 'Milk'), makeItem(2, 'Bread')]
    vi.mocked(api.reorderItems).mockRejectedValue(new api.ApiError('Could not save the order.', 400))
    vi.mocked(api.getList).mockResolvedValue({
      name: 'My shopping list',
      items: [makeItem(1, 'Milk'), makeItem(2, 'Bread')],
      spendingLimit: null,
      budgetSummary: null,
    })

    await store.moveItem(1, 1)

    // Back to the order the server has
    expect(store.items.map(item => item.id)).toEqual([1, 2])
    expect(store.errorMessage).toBe('Could not save the order.')
  })
})
