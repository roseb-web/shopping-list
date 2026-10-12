import { flushPromises, mount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import * as api from '../api/shoppingListApi'
import AddItemForm from './AddItemForm.vue'

// Fake the API call, so these tests don't need the server running
vi.mock('../api/shoppingListApi', async importOriginal => ({
  ...(await importOriginal<typeof import('../api/shoppingListApi')>()),
  addItem: vi.fn(),
}))

function mountForm() {
  return mount(AddItemForm, { global: { plugins: [createPinia()] } })
}

describe('AddItemForm', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  it('shows errors for blank fields and does not send anything', async () => {
    const wrapper = mountForm()

    await wrapper.find('form').trigger('submit')

    expect(wrapper.find('[data-test="name-error"]').text()).toBe('Enter an item name.')
    expect(wrapper.find('[data-test="price-error"]').text()).toBe('Enter a price, like 1.20.')
    expect(api.addItem).not.toHaveBeenCalled()
  })

  it('sends a valid item with the name trimmed, then clears the form', async () => {
    vi.mocked(api.addItem).mockResolvedValue({ id: 1, name: 'Milk', price: 1.2, isBought: false })
    const wrapper = mountForm()

    await wrapper.find('#item-name').setValue('  Milk  ')
    await wrapper.find('#item-price').setValue('£1.20')
    await wrapper.find('form').trigger('submit')
    await flushPromises()

    expect(api.addItem).toHaveBeenCalledWith('Milk', 1.2)
    expect((wrapper.find('#item-name').element as HTMLInputElement).value).toBe('')
    expect((wrapper.find('#item-price').element as HTMLInputElement).value).toBe('')
  })

  it("shows the server's duplicate message under the name", async () => {
    vi.mocked(api.addItem).mockRejectedValue(new api.ApiError('milk is already on your list.', 409))
    const wrapper = mountForm()

    await wrapper.find('#item-name').setValue('milk')
    await wrapper.find('#item-price').setValue('1.20')
    await wrapper.find('form').trigger('submit')
    await flushPromises()

    expect(wrapper.find('[data-test="name-error"]').text()).toBe('milk is already on your list.')
  })
})
