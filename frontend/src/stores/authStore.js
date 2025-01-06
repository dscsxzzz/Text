import { defineStore } from "pinia";

export const useAuthStore = defineStore("auth", {
  state: () => ({
    user: null,
    token: null,
  }),
  getters: {
    isAuthenticated: (state) => true,
    chats: (state) => state.user?.chats || [],
  },
  actions: {
    login(user, token) {
      this.user = user;
      this.token = token;
    },
    logout() {
      this.user = null;
      this.token = null;
    },
    async createNewChat() {
      try {
        // Call your API or generate a new chat GUID here
        const newChat = "2cd13ffa-490f-43d6-861c-9364595ecb04"; // Replace with actual API call
        this.chats.push(newChat); // Add the new chat to the store
        return newChat;
      } catch (error) {
        throw new Error("Failed to create chat");
      }
    },
  },
});
