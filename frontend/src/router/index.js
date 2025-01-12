import { createRouter, createWebHistory } from "vue-router";
import { useAuthStore } from "@/stores/authStore";
import HomePage from "@/components/HomePage.vue";
import LoginPage from "@/components/LoginPage.vue";
import ChatWindow from "@/components/ChatWindow.vue";
import MainLayout from "@/components/MainLayout.vue";
import RegisterPage from "@/components/RegisterPage.vue";

const routes = [
  {
    path: "/",
    name: "Home",
    component: HomePage,
  },
  {
    path: "/login",
    name: "Login",
    component: LoginPage,
  },
  {
    path: "/register",
    name: "Register",
    component: RegisterPage,
  },
  {
    path: "/chats/:chatGuid?",
    name: "ChatWindow",
    component: MainLayout,
    beforeEnter: async (to, from, next) => {
      const authStore = useAuthStore();
      const chatGuid = to.params.chatGuid;
  
      if (!authStore.isAuthenticated) {
        next({ name: "Login" });
        return;
      }
  
      if (chatGuid) {
        // Check if the provided chatGuid exists in user's chats
        if (!authStore.chats.some((chat) => chat === chatGuid)) {
          alert("You do not have access to this chat.");
          next({ name: "Home" });
          return;
        }
      } else {
        // If no chatGuid is provided, create a new one and navigate to it
        try {
          const newChat = await authStore.createNewChat(); // Assuming this method exists in your store to create a new chat
          next({ name: "ChatWindow", params: { chatGuid: newChat } });
          return;
        } catch (error) {
          alert("Failed to create a new chat.");
          next({ name: "Home" });
          return;
        }
      }
  
      next();
    },
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
