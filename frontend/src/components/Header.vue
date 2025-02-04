<template>
    <div class="header">
      <span class="app-title">Summary</span>
      <div class="auth-section">
        <template v-if="isLoggedIn">
          <span class="username">Welcome, {{ user.username }}</span>
          <Button 
            label="Logout" 
            class="p-button-rounded p-button-text p-button-sm logout-button"
            @click="logout"
          />
        </template>
        <template v-else>
          <Button 
            label="Login" 
            class="p-button-rounded p-button-text p-button-sm login-button"
            @click="goToLogin"
          />
        </template>
      </div>
    </div>
  </template>
  
  <script>
  import { useAuthStore } from "@/stores/authStore";
  import { Button } from "primevue";
  
  export default {
    name: "Header",
    components: { Button },
    computed: {
      isLoggedIn() {
        const authStore = useAuthStore();
        return authStore.isAuthenticated;
      },
      user() {
        const authStore = useAuthStore();
        return authStore.user || {};
      },
    },
    methods: {
      logout() {
        const authStore = useAuthStore();
        authStore.logout();
        this.$router.push("/login");
      },
      goToLogin() {
        this.$router.push("/login");
      },
    },
  };
  </script>
  
  <style scoped>
  .header {
    background-color: var(--primary-color, #343541);
    color: white;
    padding: 1rem;
    display: flex;
    align-items: center;
    justify-content: space-between;
    font-size: 1.5rem;
    font-weight: bold;
  }
  
  .auth-section {
    display: flex;
    align-items: center;
    gap: 1rem;
  }
  
  .username {
    font-size: 1rem;
    font-weight: normal;
  }
  
  .login-button, .logout-button {
    color: white;
  }
  </style>
  