import vue from '@vitejs/plugin-vue'
import { defineConfig } from 'vitest/config'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5080',
      },
    }
  },
  build: {
      outDir: '../src/ShoppingListApp.Api/wwwroot',
      emptyOutDir: true,
    },
  test: {
    globals: true,
    environment: 'jsdom',
  },
});
