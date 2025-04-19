import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import router from "./router";
import 'primeicons/primeicons.css'
import App from './App.vue'
import PrimeVue from 'primevue/config';
import { definePreset } from '@primevue/themes';
import Aura from '@primevue/themes/aura';
import ConfirmationService from 'primevue/confirmationservice';
import ToastService from 'primevue/toastservice';
import piniaPersistedstate from 'pinia-plugin-persistedstate'

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
const pinia = createPinia();
pinia.use(piniaPersistedstate);
app.use(pinia);
app.use(ToastService);
app.use(ConfirmationService);
app.mount('#app')
