<template>
  <div class="header">
    <div class="left-section">
      <Button
        icon="pi pi-bars"
        class="p-button-rounded p-button-text p-button-sm sidebar-toggle"
        @click="toggleSidebar"
        aria-label="Toggle Sidebar"
      />
      <span class="app-title">Summary</span>
    </div>
    <div class="auth-section">
      <template v-if="isLoggedIn">
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
      return authStore.user || {
        username: "user"
      };
    },
    authStore() {
      return useAuthStore();
    }
  },
  methods: {
    toggleSidebar() {
      this.authStore.toggleSidebar();
    },
    logout() {
      this.authStore.logout();
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
  background-color:  #212121;
  color: var(--p-primary-color);
  padding: 1rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-size: 1.5rem;
  font-weight: bold;
}

.left-section {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.sidebar-toggle {
  color: white;
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