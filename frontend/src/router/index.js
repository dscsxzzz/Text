import { createRouter, createWebHistory } from "vue-router";
import { useAuthStore } from "@/stores/authStore";
import HomePage from "@/components/HomePage.vue";
import LoginPage from "@/components/LoginPage.vue";
import MainLayout from "@/components/MainLayout.vue";
import RegisterPage from "@/components/RegisterPage.vue";
import TryOutWindow from "@/components/TryOutWindow.vue";
import ForgotPasswordPage from "@/components/ForgotPasswordPage.vue";

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
        console.log(authStore.isAuthenticated);
        next({ name: "Login" });
        return;
      }
  
      if (chatGuid) {
        if (!authStore.chats.some((chat) => chat.chatId === chatGuid)) {
          alert("You do not have access to this chat.");
          next({ name: "Home" });
          return;
        }
      } else {
        try {
          if(authStore.chats.length != 0)
          {
            console.log(authStore.chats); 
            let chatToRedirectTo = authStore.chats[0].chatId
            next({ name: "ChatWindow", params: { chatGuid: chatToRedirectTo } });
            return;
          }
          const newChat = await authStore.createNewChat();
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
  {
    path: "/try-out",
    name: "TryOut",
    component: TryOutWindow,
  },
  {
    path: "/forgot-password",
    name: "ForgotPassword",
    component: ForgotPasswordPage,
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
