<script>
import SignalRTest from './components/SignalRTest.vue'
import MainLayout from './components/MainLayout.vue';
import HomePage from './components/HomePage.vue';
import { RouterView } from 'vue-router';
export default {
  components: {
    SignalRTest,
    MainLayout,
    HomePage
  },
};
</script>

<template>
  <RouterView/>
</template>

<style scoped>
</style>
