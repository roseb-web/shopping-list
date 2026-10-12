import type { Budget, Item, ShoppingList } from '../types'

// In development, Vite's proxy sends /api calls on to the .NET API (see vite.config.ts)
const baseUrl = '/api/list'

// An error the API sent back on purpose, like 409 for a duplicate
export class ApiError extends Error {
  readonly status: number

  constructor(message: string, status: number) {
    super(message)
    this.status = status
  }
}

// Calls the API and returns the JSON body. Throws an ApiError if the API says no.
async function request<T>(path: string, method = 'GET', body?: unknown): Promise<T> {
  const response = await fetch(baseUrl + path, {
    method,
    headers: body === undefined ? undefined : { 'Content-Type': 'application/json' },
    body: body === undefined ? undefined : JSON.stringify(body),
  })

  if (!response.ok) {
    // Errors come back as problem details. The friendly message is in "detail".
    const problem = await response.json().catch(() => null)
    throw new ApiError(problem?.detail ?? 'Something went wrong. Please try again.', response.status)
  }

  // 204 No Content has no body to read
  if (response.status === 204) {
    return undefined as T
  }

  return (await response.json()) as T
}

// Turns any error into text that's safe to show on screen
export function messageFrom(error: unknown): string {
  if (error instanceof ApiError) {
    return error.message
  }

  // fetch throws a TypeError when it can't reach the server at all
  if (error instanceof TypeError) {
    return "Can't reach the server. Check your connection and try again."
  }

  return 'Something went wrong. Please try again.'
}

export function getList(): Promise<ShoppingList> {
  return request<ShoppingList>('')
}

export function addItem(name: string, price: number): Promise<Item> {
  return request<Item>('/items', 'POST', { name, price })
}

export function removeItem(id: number): Promise<void> {
  return request<void>(`/items/${id}`, 'DELETE')
}

export function setBought(id: number, isBought: boolean): Promise<Item> {
  return request<Item>(`/items/${id}/bought`, 'PATCH', { isBought })
}

export function reorderItems(itemIds: number[]): Promise<void> {
  return request<void>('/items/order', 'PUT', { itemIds })
}

export function setSpendingLimit(spendingLimit: number | null): Promise<Budget> {
  return request<Budget>('/limit', 'PUT', { spendingLimit })
}
