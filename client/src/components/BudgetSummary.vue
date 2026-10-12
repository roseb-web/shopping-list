<script setup lang="ts">
import { computed, nextTick, ref } from 'vue'
import { messageFrom } from '../api/shoppingListApi'
import { useShoppingListStore } from '../stores/shoppingList'
import { formatMoney, parsePrice } from '../utils/money'

const MAX_SPENDING_LIMIT = 10_000 // Same as ItemRules.MaxSpendingLimit

const store = useShoppingListStore()

const isEditing = ref(false)
const limitText = ref('')
const limitError = ref('')
const isSaving = ref(false)
const limitInput = ref<HTMLInputElement | null>(null)

// How full the progress bar is, from 0 to 100
const percentUsed = computed(() => {
  const { total, spendingLimit } = store.budget

  if (spendingLimit === null) {
    return 0
  }
  if (spendingLimit === 0) {
    return total > 0 ? 100 : 0
  }

  return Math.min(100, Math.round((total / spendingLimit) * 100))
})

async function startEditing() {
  limitText.value = store.spendingLimit === null ? '' : store.spendingLimit.toFixed(2)
  limitError.value = ''
  isEditing.value = true

  // Wait for Vue to show the input before moving focus to it
  await nextTick()
  limitInput.value?.focus()
}

async function onSubmit() {
  limitError.value = ''

  // An empty box means "no limit"
  let limit: number | null = null

  if (limitText.value.trim() !== '') {
    limit = parsePrice(limitText.value)

    if (limit === null) {
      limitError.value = 'Enter an amount, like 50.00, or leave it empty for no limit.'
      return
    }
    if (limit > MAX_SPENDING_LIMIT) {
      limitError.value = `The limit must be £${MAX_SPENDING_LIMIT.toLocaleString('en-GB')} or less.`
      return
    }
  }

  isSaving.value = true

  try {
    await store.setSpendingLimit(limit)
    isEditing.value = false
  } catch (error) {
    limitError.value = messageFrom(error)
  } finally {
    isSaving.value = false
  }
}
</script>

<template>
  <!-- sticky-top keeps the budget in view while scrolling a long list -->
  <section class="card sticky-top budget_bar" aria-label="Budget">
    <div class="card-body">
      <div class="d-flex justify-content-between align-items-center gap-2">
        <div>
          <div class="text-secondary small">Total cost</div>
          <div class="fs-3 fw-bold" data-test="budget-total">{{ formatMoney(store.budget.total) }}</div>
        </div>

        <button v-if="!isEditing" type="button" class="btn btn-outline-primary btn-sm" data-test="limit-button"
          @click="startEditing">
          {{ store.spendingLimit === null ? 'Set limit' : `Limit: ${formatMoney(store.spendingLimit)}` }}
        </button>
      </div>

      <form v-if="isEditing" class="mt-3" novalidate @submit.prevent="onSubmit">
        <label for="spending-limit" class="form-label">Spending limit (£)</label>
        <div class="d-flex gap-2 align-items-start">
          <div class="flex-grow-1">
            <input id="spending-limit" ref="limitInput" v-model="limitText" type="text" inputmode="decimal"
              class="form-control" :class="{ 'is-invalid': limitError }"
              :aria-describedby="limitError ? 'limit-error' : 'limit-hint'" autocomplete="off" />
            <div v-if="limitError" id="limit-error" class="invalid-feedback" data-test="limit-error">
              {{ limitError }}
            </div>
            <div v-else id="limit-hint" class="form-text">Leave empty for no limit.</div>
          </div>
          <button type="submit" class="btn btn-primary" :disabled="isSaving">Save</button>
          <button type="button" class="btn btn-outline-secondary" @click="isEditing = false">Cancel</button>
        </div>
      </form>

      <template v-if="store.budget.spendingLimit !== null">
        <div class="progress mt-3" role="progressbar" aria-label="Share of spending limit used"
          :aria-valuenow="percentUsed" aria-valuemin="0" aria-valuemax="100">
          <div class="progress-bar" :class="store.budget.isOverBudget ? 'bg-danger' : 'bg-success'"
            :style="{ width: `${percentUsed}%` }"></div>
        </div>
      </template>

      <!-- aria-live: screen readers announce changes here, like going over budget -->
      <div aria-live="polite">
        <div v-if="store.budget.isOverBudget" class="alert alert-danger mt-3 mb-0 py-2" data-test="over-budget">
          <strong>Over budget</strong> by {{ formatMoney(-(store.budget.remaining ?? 0)) }}
        </div>
        <p v-else-if="store.budget.remaining !== null" class="text-success mt-2 mb-0" data-test="money-left">
          {{ formatMoney(store.budget.remaining) }} left to spend
        </p>
      </div>
    </div>
  </section>
</template>
