<template>
  <div class="sidebar">
    <p-panel header="Chats" class="mb-3" style="background-color: var(--sidebar-bg, #202123); color: white;">
      <div class="new-chat">
        <p-button
          label="New Chat"
          icon="pi pi-plus"
          class="p-button-outlined p-button-rounded mb-3"
          @click="startNewChat"
        />
      </div>
      <ul class="chat-list">
        <li v-for="chat in chats" :key="chat.chatId" class="chat-item">
          <div class="chat-content">
            <p-input-text
              v-if="editingChatId === chat.chatId"
              v-model="chat.name"
              @blur="saveChat(chat)"
              @keyup.enter="saveChat(chat)"
              class="chat-input"
            />
            <div v-else class="chat-title" @click="selectChat(chat)">
              <span>{{ chat.name }}</span>
            </div>
            <p-button
              v-if="editingChatId !== chat.chatId"
              icon="pi pi-pencil"
              class="p-button-rounded p-button-text p-button-sm"
              @click="editChat(chat.chatId)"
              style="color: var(--hover-text, #66ccff);"
            />
          </div>
        </li>
      </ul>
    </p-panel>
  </div>
</template>

<script>
import { Panel, Button, InputText } from "primevue";
import ApiService from "@/ApiService"; // Import ApiService
import { useAuthStore } from "@/stores/authStore";

export default {
  components: {
    "p-panel": Panel,
    "p-button": Button,
    "p-input-text": InputText,
  },
  data() {
    return {
      chats: [],
      editingChatId: null,
      authStore: null
    };
  },
  methods: {
    async fetchChats() {
      try {
        console.log(this.authStore);
        const response = await ApiService.getUserChats(this.authStore.User.userId);
        this.chats = response;
      } catch (error) {
        console.error("Error fetching chats:", error);
      }
    },
    async startNewChat() {
      try {
        const newChat = await this.authStore.createNewChat(); // Create a new chat via API
        this.chats = this.authStore.chats; // Add the new chat to the local list
      } catch (error) {
        console.error("Error creating a new chat:", error);
      }
    },
    editChat(chatId) {
      this.editingChatId = chatId; // Set the chat to be edited
    },
    async saveChat(chat) {
      try {
        this.editingChatId = null; // Exit edit mode
        await ApiService.updateChat(this.authStore.User.userId, { name: chat.name, chatId: chat.chatId }); // Update chat title via API
        console.log("Chat updated:", chat);
      } catch (error) {
        console.error("Error updating chat:", error);
      }
    },
    selectChat(chat) {
      this.$router.push(`/chats/${chat.chatId}`); // Navigate to the chat details page
    },
  },
  mounted() {
    this.authStore = useAuthStore();
    this.fetchChats(); // Fetch chats on component mount
  }
};
</script>

<style scoped>
.sidebar {
  background-color: var(--sidebar-bg, #202123);
  color: white;
  width: 300px;
  padding: 1rem;
  overflow-y: auto;
  border-radius: 8px;
}

.chat-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.chat-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.5rem 0;
  border-bottom: 1px solid var(--hover-bg, #343541);
  transition: background-color 0.3s;
}

.chat-content {
  display: flex;
  align-items: center;
  width: 100%;
}

.chat-title {
  cursor: pointer;
  padding: 0.5rem;
  flex: 1;
  font-size: 1rem;
  font-weight: 500;
  color: white;
  transition: color 0.3s;
}

.chat-title:hover {
  color: var(--hover-text, #66ccff);
}

.chat-input {
  flex: 1;
  background-color: var(--input-bg, #3a3a3a);
  border: none;
  color: white;
  padding: 0.3rem;
  border-radius: 4px;
}

.new-chat {
  text-align: center;
}

.chat-item:hover {
  background-color: var(--hover-bg, #343541);
}

.chat-item p-button {
  margin-left: 0.5rem;
}
</style>
