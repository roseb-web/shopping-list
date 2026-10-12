<script setup lang="ts">
import { ref } from 'vue'
import { ApiError, messageFrom } from '../api/shoppingListApi'
import { useShoppingListStore } from '../stores/shoppingList'
import { parsePrice } from '../utils/money'

const NAME_MAX_LENGTH = 100 // Same as ItemRules.NameMaxLength
const MAX_PRICE = 10_000 // Same as ItemRules.MaxPrice

const store = useShoppingListStore()

// v-model links these to the inputs, both ways
const name = ref('')
const priceText = ref('')

const nameError = ref('')
const priceError = ref('')
const formError = ref('')
const isSaving = ref(false)

// A "template ref": lets us reach the real <input> element, to move focus back to it
const nameInput = ref<HTMLInputElement | null>(null)

// Quick checks in the browser, so the shopper doesn't wait for the server.
// The server checks again, and its checks are the ones that count.
function validate(): number | null {
  nameError.value = ''
  priceError.value = ''
  formError.value = ''

  const trimmedName = name.value.trim()
  if (trimmedName === '') {
    nameError.value = 'Enter an item name.'
  } else if (trimmedName.length > NAME_MAX_LENGTH) {
    nameError.value = `Item name must be ${NAME_MAX_LENGTH} characters or fewer.`
  }

  const price = parsePrice(priceText.value)
  if (price === null) {
    priceError.value = 'Enter a price, like 1.20.'
  } else if (price > MAX_PRICE) {
    priceError.value = `Price must be £${MAX_PRICE.toLocaleString('en-GB')} or less.`
  }

  return nameError.value || priceError.value ? null : price
}

async function onSubmit() {
  const price = validate()
  if (price === null) {
    return
  }

  isSaving.value = true

  try {
    await store.addItem(name.value.trim(), price)

    // Ready for the next item
    name.value = ''
    priceText.value = ''
    nameInput.value?.focus()
  } catch (error) {
    if (error instanceof ApiError && error.status === 409) {
      // Duplicate: show it next to the name, where the problem is
      nameError.value = error.message
    } else {
      formError.value = messageFrom(error)
    }
  } finally {
    isSaving.value = false
  }
}
</script>

<template>
  <section class="card" aria-labelledby="add-heading">
    <div class="card-body">
      <h2 id="add-heading" class="h5">Add an item</h2>

      <!-- .prevent stops the browser reloading the page, like a WebForms postback would -->
      <form novalidate @submit.prevent="onSubmit">
        <div class="row g-2">
          <div class="col-12 col-sm-7">
            <label for="item-name" class="form-label">Item name</label>
            <input id="item-name" ref="nameInput" v-model="name" type="text" class="form-control"
              :class="{ 'is-invalid': nameError }" :aria-describedby="nameError ? 'name-error' : undefined"
              autocomplete="off" />
            <div v-if="nameError" id="name-error" class="invalid-feedback" data-test="name-error">
              {{ nameError }}
            </div>
          </div>

          <div class="col-7 col-sm-3">
            <label for="item-price" class="form-label">Price (£)</label>
            <input id="item-price" v-model="priceText" type="text" inputmode="decimal" class="form-control"
              :class="{ 'is-invalid': priceError }" :aria-describedby="priceError ? 'price-error' : undefined"
              autocomplete="off" />
            <div v-if="priceError" id="price-error" class="invalid-feedback" data-test="price-error">
              {{ priceError }}
            </div>
          </div>

          <div class="col-5 col-sm-2">
            <!-- Invisible spacer the same height as a label, so the button lines up with the inputs -->
            <div class="form-label invisible" aria-hidden="true">Add</div>
            <button type="submit" class="btn btn-primary w-100 add_button" :disabled="isSaving">Add</button>
          </div>
        </div>

        <div v-if="formError" class="alert alert-danger mt-3 mb-0" role="alert" data-test="form-error">
          {{ formError }}
        </div>
      </form>
    </div>
  </section>
</template>
