import { createApp } from 'vue'
import { Quasar, Notify } from 'quasar'
import quasarLangFr from 'quasar/lang/fr'
import '@quasar/extras/material-icons/material-icons.css'
import 'quasar/dist/quasar.css'
import App from './App.vue'

createApp(App)
  .use(Quasar, { lang: quasarLangFr, plugins: { Notify } }) // textes de Quasar en français
  .mount('#app')
