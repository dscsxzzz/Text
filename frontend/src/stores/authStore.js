import ApiService from "@/ApiService";
import { defineStore } from "pinia";

export const useAuthStore = defineStore("auth", {
  state: () => ({
    user: null,
    token: null,
  }),
  getters: {
    isAuthenticated: (state) => !!state.token,
    chats: (state) => state.user?.chats || [],
  },
  actions: {
    async login(credentials) {
      try {
        const data = await ApiService.login(credentials);
        this.user = await ApiService.getUserProfile(data.userId); // Assuming userId is part of the login response
        this.token = data.access_token;
        ApiService.setAuthToken(this.token);
        return true;
      } catch (error) {
        console.error("Login failed:", error);
        return false;
      }
    },

    logout() {
      this.user = null;
      this.token = null;
      ApiService.setAuthToken(null);
    },

    async createNewChat() {
      if (!this.isAuthenticated || !this.user) {
        throw new Error("You must be logged in to create a chat.");
      }
      try {
        const userId = this.user.id; // Adjust based on your user object structure
        const newChat = await ApiService.createChat(userId, { /* Add necessary payload here */ });
        this.user.chats = [...this.chats, newChat]; // Update local state
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
