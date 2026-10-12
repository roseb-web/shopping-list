<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { VueDraggable, type DraggableEvent } from 'vue-draggable-plus'
import { useShoppingListStore } from '../stores/shoppingList'
import { formatMoney } from '../utils/money'
import ShoppingListItem from './ShoppingListItem.vue'

const store = useShoppingListStore()

// storeToRefs lets v-model below write the new order straight back into the store
const { items } = storeToRefs(store)

function onDragEnd(event: DraggableEvent) {
  // Only save if the item actually moved
  if (event.oldIndex !== event.newIndex) {
    store.saveOrder()
  }
}
</script>

<template>
  <section class="card" aria-labelledby="list-heading">
    <div class="card-header d-flex justify-content-between align-items-center">
      <h2 id="list-heading" class="h5 mb-0">My list</h2>
      <span class="text-secondary small" data-test="items-to-get">
        {{ store.itemsToGet }} of {{ items.length }} to get
      </span>
    </div>

    <p v-if="items.length === 0" class="card-body text-secondary mb-0">
      Your list is empty. Add your first item above.
    </p>

    <!-- delay + delayOnTouchOnly: on a phone, press and hold briefly before dragging -->
    <VueDraggable v-model="items" tag="ul" class="list-group list-group-flush" handle=".drag_handle" :delay="150"
      :delay-on-touch-only="true" @end="onDragEnd">
      <ShoppingListItem v-for="(item, index) in items" :key="item.id" :item="item" :is-first="index === 0"
        :is-last="index === items.length - 1" />
    </VueDraggable>

    <div class="card-footer d-flex justify-content-between fw-semibold">
      <span>Total</span>
      <span data-test="list-total">{{ formatMoney(store.budget.total) }}</span>
    </div>
  </section>
</template>
