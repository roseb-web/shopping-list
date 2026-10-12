const formatter = new Intl.NumberFormat('en-GB', { style: 'currency', currency: 'GBP' })

// 1.2 -> "£1.20"
export function formatMoney(amount: number): string {
  return formatter.format(amount)
}

// 1.2 -> 120. Working in whole pence avoids JavaScript giving 0.1 + 0.2 = 0.30000000000000004
export function toPence(amount: number): number {
  return Math.round(amount * 100)
}

// Adds prices in pence, then turns the total back into pounds
export function sumPrices(prices: number[]): number {
  const totalPence = prices.reduce((total, price) => total + toPence(price), 0)
  return totalPence / 100
}

// Reads what someone typed into a price box.
// Allows "1", "1.2", "1.20" and "£1.20". Returns null if it isn't a valid price.
export function parsePrice(text: string): number | null {
  const cleaned = text.trim().replace(/^£/, '')

  if (!/^\d+(\.\d{1,2})?$/.test(cleaned)) {
    return null
  }

  return Number(cleaned)
}
