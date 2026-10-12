import { describe, expect, it } from 'vitest'
import { formatMoney, parsePrice, sumPrices, toPence } from './money'

describe('formatMoney', () => {
  it('shows pounds with two decimal places', () => {
    expect(formatMoney(1.2)).toBe('£1.20')
    expect(formatMoney(0)).toBe('£0.00')
    expect(formatMoney(1234.5)).toBe('£1,234.50')
  })
})

describe('toPence', () => {
  it('turns pounds into whole pence', () => {
    expect(toPence(1.2)).toBe(120)
    expect(toPence(0.29)).toBe(29)
  })
})

describe('sumPrices', () => {
  it('adds 0.1 and 0.2 to exactly 0.3', () => {
    expect(sumPrices([0.1, 0.2])).toBe(0.3)
  })

  it('is 0 for no prices', () => {
    expect(sumPrices([])).toBe(0)
  })
})

describe('parsePrice', () => {
  it.each([
    ['1', 1],
    ['1.2', 1.2],
    ['1.20', 1.2],
    ['£1.20', 1.2],
    [' 0.80 ', 0.8],
  ])('reads "%s" as %d', (text, expected) => {
    expect(parsePrice(text)).toBe(expected)
  })

  it.each(['', 'abc', '-1', '1.234', '1.', '£', '1,20'])('rejects "%s"', text => {
    expect(parsePrice(text)).toBeNull()
  })
})
