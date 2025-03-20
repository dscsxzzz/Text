import ApiService from "@/ApiService";
import { defineStore } from "pinia";

export const useAuthStore = defineStore("auth", {
  state: () => ({
    user: null,
    token: null,
    forgotPasswordToken: null,
    confirmEmailToken: null,
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
        const newChat = await ApiService.createChat(userId, {name: `chat ${this.user.chats.length}`});
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
        const data = await ApiService.register(credentials); 
        this.confirmEmailToken = data.data.token;
        return data; // Return true if registration and login are successful
      } catch (error) {
        console.error("Registration failed:", error);
        return data; // Return false if the registration fails
      }
    },

    async confirmEmail(code) {
      try {
        const data = await ApiService.confirmEmail(this.confirmEmailToken, code); 
        return data;
      } catch (error) {
        console.error("Registration failed:", error);
        return data; // Return false if the registration fails
      }
    },

    async forgotPassword(credentials) {
      try {
        const token = await ApiService.forgotPassword(credentials); // API call to register a new user
        this.forgotPasswordToken = token;
        return true; // Return true if registration and login are successful
      } catch (error) {
        console.error("Reseting Password failed:", error);
        return false; // Return false if the registration fails
      }
    },

    async verifyResetCode(credentials) {
      try {
        const data = await ApiService.verifyResetCode(this.forgotPasswordToken, credentials); // API call to register a new user
        return data; // Return true if registration and login are successful
      } catch (error) {
        console.error("Reseting Password failed:", error);
        return data; // Return false if the registration fails
      }
    },

    async resetPassword(credentials) {
      try {
        const data = await ApiService.resetPassowrd(this.forgotPasswordToken, credentials); // API call to register a new user
        return data; // Return true if registration and login are successful
      } catch (error) {
        console.error("Reseting Password failed:", error);
        return data; // Return false if the registration fails
      }
    },
  },
});
