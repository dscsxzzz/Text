import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import router from "./router";
import 'primeicons/primeicons.css'
import App from './App.vue'
import PrimeVue from 'primevue/config';
import Aura from '@primevue/themes/aura';

const app = createApp(App)
app.use(router);
app.use(PrimeVue, {
    theme: {
        preset: Aura
    }
});
app.use(createPinia())

app.mount('#app')
