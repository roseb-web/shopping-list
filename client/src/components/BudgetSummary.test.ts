import { mount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import { beforeEach, describe, expect, it } from 'vitest'
import { useShoppingListStore } from '../stores/shoppingList'
import BudgetSummary from './BudgetSummary.vue'

// Sets up the store with some prices and a limit, then shows the component
function mountWith(prices: number[], spendingLimit: number | null) {
  const pinia = createPinia()
  setActivePinia(pinia)

  const store = useShoppingListStore()
  store.items = prices.map((price, index) => ({ id: index + 1, name: `Item ${index + 1}`, price, isBought: false }))
  store.spendingLimit = spendingLimit

  return mount(BudgetSummary, { global: { plugins: [pinia] } })
}

describe('BudgetSummary', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  it('shows the total', () => {
    const wrapper = mountWith([1.2, 0.8], null)

    expect(wrapper.find('[data-test="budget-total"]').text()).toBe('£2.00')
  })

  it('offers "Set limit" when there is no limit', () => {
    const wrapper = mountWith([1.2], null)

    expect(wrapper.find('[data-test="limit-button"]').text()).toBe('Set limit')
    expect(wrapper.find('[data-test="money-left"]').exists()).toBe(false)
  })

  it('shows the money left when under the limit', () => {
    const wrapper = mountWith([1.2, 0.8], 5)

    expect(wrapper.find('[data-test="money-left"]').text()).toBe('£3.00 left to spend')
    expect(wrapper.find('[data-test="over-budget"]').exists()).toBe(false)
  })

  it('warns when over the limit', () => {
    const wrapper = mountWith([1.2, 0.8], 1)

    expect(wrapper.find('[data-test="over-budget"]').text()).toBe('Over budget by £1.00')
    expect(wrapper.find('.progress-bar').classes()).toContain('bg-danger')
  })
})
