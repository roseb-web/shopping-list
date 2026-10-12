<script setup lang="ts">
import { useShoppingListStore } from '../stores/shoppingList'
import type { Item } from '../types'
import { formatMoney } from '../utils/money'

// Props are values the parent component passes in, like attributes on an HTML tag
const props = defineProps<{
  item: Item
  isFirst: boolean
  isLast: boolean
}>()

const store = useShoppingListStore()

function onBoughtChange(event: Event) {
  const isBought = (event.target as HTMLInputElement).checked
  store.setBought(props.item.id, isBought)
}
</script>

<template>
  <li class="list-group-item d-flex align-items-center gap-2" :class="{ item_bought: item.isBought }" data-test="item">
    <!-- Only this grip starts a drag, so the list can still scroll on a phone -->
    <button type="button" class="btn btn-sm btn-link text-secondary drag_handle px-1"
      :aria-label="`Drag to reorder ${item.name}`">
      <span aria-hidden="true">⠿</span>
    </button>

    <div class="form-check flex-grow-1 mb-0 text-break">
      <input :id="`bought-${item.id}`" class="form-check-input" type="checkbox" :checked="item.isBought"
        @change="onBoughtChange" />
      <label :for="`bought-${item.id}`" class="form-check-label item_name">{{ item.name }}</label>
    </div>

    <span class="item_price text-nowrap">{{ formatMoney(item.price) }}</span>

    <!-- Desktop: buttons you can see -->
    <div class="d-none d-md-inline-flex gap-1">
      <button type="button" class="btn btn-sm btn-outline-secondary" :disabled="isFirst"
        :aria-label="`Move ${item.name} up`" @click="store.moveItem(item.id, -1)">
        <span aria-hidden="true">↑</span>
      </button>
      <button type="button" class="btn btn-sm btn-outline-secondary" :disabled="isLast"
        :aria-label="`Move ${item.name} down`" @click="store.moveItem(item.id, 1)">
        <span aria-hidden="true">↓</span>
      </button>
      <button type="button" class="btn btn-sm btn-outline-danger" :aria-label="`Remove ${item.name}`"
        @click="store.removeItem(item.id)">
        Remove
      </button>
    </div>

    <!-- Mobile: the same actions in a ⋯ menu, to save space -->
    <div class="dropdown d-md-none">
      <button type="button" class="btn btn-sm btn-outline-secondary" data-bs-toggle="dropdown" aria-expanded="false"
        :aria-label="`More actions for ${item.name}`">
        <span aria-hidden="true">⋯</span>
      </button>
      <ul class="dropdown-menu dropdown-menu-end">
        <li>
          <button type="button" class="dropdown-item" :disabled="isFirst" @click="store.moveItem(item.id, -1)">
            Move up
          </button>
        </li>
        <li>
          <button type="button" class="dropdown-item" :disabled="isLast" @click="store.moveItem(item.id, 1)">
            Move down
          </button>
        </li>
        <li>
          <button type="button" class="dropdown-item text-danger" @click="store.removeItem(item.id)">
            Remove
          </button>
        </li>
      </ul>
    </div>
  </li>
</template>
