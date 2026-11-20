import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { quasar } from '@quasar/vite-plugin'

// Le proxy envoie les appels /api vers l'API .NET (évite les problèmes de CORS en développement)
export default defineConfig({
  plugins: [vue(), quasar()],
  server: {
    port: 5173,
    proxy: {
      '/api': { target: 'https://localhost:7216', secure: false }
    }
  }
})
