<script setup lang="ts">
import { onMounted } from 'vue'
import AddItemForm from './components/AddItemForm.vue'
import BudgetSummary from './components/BudgetSummary.vue'
import ShoppingListItems from './components/ShoppingListItems.vue'
import { useShoppingListStore } from './stores/shoppingList'

const store = useShoppingListStore()

// Load the list once, when the page first opens
onMounted(() => {
  store.load()
})
</script>

<template>
  <nav class="navbar navbar-dark bg-dark">
    <div class="container">
      <span class="navbar-brand mb-0 h1">Shopping List</span>
    </div>
  </nav>

  <main class="container py-3">
    <div class="col-lg-8 mx-auto d-flex flex-column gap-3">
      <div v-if="store.errorMessage" class="alert alert-danger alert-dismissible mb-0" role="alert"
        data-test="error-message">
        {{ store.errorMessage }}
        <button type="button" class="btn-close" aria-label="Close" @click="store.errorMessage = ''"></button>
      </div>

      <p v-if="store.isLoading && store.items.length === 0" class="text-secondary mb-0">Loading…</p>

      <template v-else>
        <BudgetSummary />
        <AddItemForm />
        <ShoppingListItems />
      </template>
    </div>
  </main>
</template>
