import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import router from "./router";
import 'primeicons/primeicons.css'
import App from './App.vue'
import PrimeVue from 'primevue/config';
import { definePreset } from '@primevue/themes';
import Aura from '@primevue/themes/aura';

const app = createApp(App)

const MyPreset = definePreset(Aura, {
    components: {
        password: {
            input: {
                width: "100%"
            }
        }
    }
});

app.use(router);
app.use(PrimeVue, {
    theme: {
        preset: MyPreset
    }
});
app.use(createPinia())

app.mount('#app')
