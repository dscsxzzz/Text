import ApiService from "@/ApiService";
import { defineStore } from "pinia";

export const useAuthStore = defineStore("auth", {
  state: () => ({
    user: null,
    token: null,
  }),
  getters: {
    isAuthenticated: (state) => !!state.token,
    User: (state) => state.user,
    chats: (state) => state.user?.chats || [],
  },
  actions: {
    async login(credentials) {
      try {
        const data = await ApiService.login(credentials);
        this.token = data.access_token;
        console.log(this.isAuthenticated);
        ApiService.setAuthToken(this.token);
        this.user = await ApiService.getUserProfile(data.userId);
        return true;
      } catch (error) {
        console.error("Login failed:", error);
        return false;
      }
    },

    logout() {
      this.user = null;
      ApiService.setAuthToken(null);
    },

    async createNewChat() {
      if (!this.isAuthenticated || !this.user) {
        throw new Error("You must be logged in to create a chat.");
      }
      try {
        const userId = this.user.userId; // Adjust based on your user object structure
        const newChat = await ApiService.createChat(userId, {name: "some name"});
        this.user.chats = await ApiService.getUserChats(userId);
        return newChat;
      } catch (error) {
        console.error("Failed to create chat:", error);
        throw error;
      }
    },

    // Add register method for user registration
    async register(credentials) {
      try {
        const data = await ApiService.register(credentials); // API call to register a new user
        return data; // Return true if registration and login are successful
      } catch (error) {
        console.error("Registration failed:", error);
        return false; // Return false if the registration fails
      }
    },
  },
});
