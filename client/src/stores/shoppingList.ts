import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import * as api from '../api/shoppingListApi'
import { messageFrom } from '../api/shoppingListApi'
import type { Item } from '../types'
import { getBudget } from '../utils/budget'

export const useShoppingListStore = defineStore('shoppingList', () => {
  // State
  const items = ref<Item[]>([])
  const spendingLimit = ref<number | null>(null)
  const isLoading = ref(false)
  const errorMessage = ref('')

  // Computed: these update themselves when items or spendingLimit change
  const itemsToGet = computed(() => items.value.filter(item => !item.isBought).length)
  const budget = computed(() => getBudget(items.value.map(item => item.price), spendingLimit.value))

  // Story 1: load the list from the API
  async function load() {
    isLoading.value = true
    errorMessage.value = ''

    try {
      const list = await api.getList()
      items.value = list.items
      spendingLimit.value = list.spendingLimit
    } catch (error) {
      errorMessage.value = messageFrom(error)
    } finally {
      isLoading.value = false
    }
  }

  // If an optimistic change couldn't be saved, put the screen back to what the server has
  async function reloadAfterFailedSave(error: unknown) {
    await load()
    errorMessage.value = messageFrom(error)
  }

  // Story 2. Not optimistic: the form waits for the server, and shows its error if it fails.
  // That's why this throws instead of setting errorMessage.
  async function addItem(name: string, price: number) {
    const item = await api.addItem(name, price)
    items.value.push(item)
  }

  // Story 3. Optimistic: remove it from the screen first, then save
  async function removeItem(id: number) {
    items.value = items.value.filter(item => item.id !== id)

    try {
      await api.removeItem(id)
    } catch (error) {
      await reloadAfterFailedSave(error)
    }
  }

  // Story 4. Optimistic, like removeItem
  async function setBought(id: number, isBought: boolean) {
    const item = items.value.find(item => item.id === id)
    if (!item) {
      return
    }

    item.isBought = isBought

    try {
      await api.setBought(id, isBought)
    } catch (error) {
      await reloadAfterFailedSave(error)
    }
  }

  // Story 6: save the current order. Called after a drag, or after moveItem
  async function saveOrder() {
    try {
      await api.reorderItems(items.value.map(item => item.id))
    } catch (error) {
      await reloadAfterFailedSave(error)
    }
  }

  // Story 6, for the up/down buttons. direction is -1 for up, 1 for down
  async function moveItem(id: number, direction: -1 | 1) {
    const index = items.value.findIndex(item => item.id === id)
    const newIndex = index + direction

    // Already at the top or bottom (or not found), so there's nothing to do
    if (index === -1 || newIndex < 0 || newIndex >= items.value.length) {
      return
    }

    const [item] = items.value.splice(index, 1)
    items.value.splice(newIndex, 0, item!)

    await saveOrder()
  }

  // Story 8. Like addItem, this throws so the limit form can show the error.
  // null removes the limit.
  async function setSpendingLimit(limit: number | null) {
    await api.setSpendingLimit(limit)
    spendingLimit.value = limit
  }

  // Anything returned here can be used by components
  return {
    items,
    spendingLimit,
    isLoading,
    errorMessage,
    itemsToGet,
    budget,
    load,
    addItem,
    removeItem,
    setBought,
    saveOrder,
    moveItem,
    setSpendingLimit,
  }
})
