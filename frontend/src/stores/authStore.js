import ApiService from "@/ApiService";
import { defineStore } from "pinia";

export const useAuthStore = defineStore("auth", {
  state: () => ({
    user: null,
    token: null,
    forgotPasswordToken: null,
    confirmEmailToken: null,
    isSidebarVisible: false,
    lastSaved: null,
  }),
  getters: {
    isAuthenticated: (state) => !!state.token,
    User: (state) => state.user,
    chats: (state) => state.user?.chats || [],
    sidebarVisible: (state) => state.isSidebarVisible
  },
  
  actions: {
    async login(credentials) {
      try {
        const data = await ApiService.login(credentials);
        this.token = data.access_token;
        this.lastSaved = Date.now();
        console.log(this.isAuthenticated);
        ApiService.setAuthToken(this.token);
        this.user = await ApiService.getUserProfile(data.userId);
        return true;
      } catch (error) {
        console.error("Login failed:", error);
        return false;
      }
    },

    checkExpiration() {
      const now = Date.now()
      const fifteenMinutes = 15 * 60 * 1000

      if (this.lastSaved && now - this.lastSaved > fifteenMinutes) {
        this.clearAll();
        return;
      }
      ApiService.setAuthToken(this.token);
    },

    clearAll()
    {
      this.user = null,
      this.token = null,
      this.forgotPasswordToken = null,
      this.confirmEmailToken = null,
      this.isSidebarVisible = false,
      this.lastSaved = null
    },
    
    toggleSidebar() {
      this.isSidebarVisible = !this.isSidebarVisible;
    },

    logout() {
      this.clearAll();
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
        if(data.status === 200)
        {
          this.confirmEmailToken = data.data.token;
        }
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
        throw error.message;
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

  persist: {
    storage: localStorage,
  }
});
