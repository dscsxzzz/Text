<template>
  <Drawer v-model:visible="authStore.isSidebarVisible" header="Chats" class="custom-drawer">
    <p-panel>
      <div class="new-chat">
        <p-button
          label="New Chat"
          icon="pi pi-plus"
          class="p-button-outlined p-button-rounded mb-3 new-chat-button"
          @click="startNewChat"
        />
      </div>
      <Divider />
      <ul class="chat-list">
        <li
          v-for="chat in chats"
          :key="chat.chatId"
          :class="['chat-item', { 'active': chat.chatId === route.params.chatGuid }]"
        >
          <div class="chat-content">
            <p-input-text
              v-if="editingChatId === chat.chatId"
              v-model="chat.name"
              @blur="saveChat(chat)"
              @keyup.enter="saveChat(chat)"
              class="chat-input"
              autofocus
            />
            <div v-else class="chat-title" @click="selectChat(chat)">
              <i class="pi pi-comments chat-icon"></i>
              <span class="chat-name">{{ chat.name }}</span>
            </div>
            <div class="chat-actions" v-if="editingChatId !== chat.chatId">
              <p-button
                icon="pi pi-pencil"
                class="p-button-rounded p-button-text p-button-sm edit-button"
                @click.stop="editChat(chat.chatId)"
                v-tooltip.top="'Rename'"
              />
            </div>
          </div>
        </li>
      </ul>
      <div v-if="chats.length === 0" class="no-chats">
        <i class="pi pi-inbox no-chat-icon"></i>
        <p>No chats yet</p>
      </div>
    </p-panel>
  </Drawer>
</template>

<script>
import { Panel, Button, InputText, Divider, Tooltip } from "primevue";
import Drawer from 'primevue/drawer';
import ApiService from "@/ApiService";
import { useAuthStore } from "@/stores/authStore";
import { useRoute } from "vue-router";

export default {
  components: {
    "p-panel": Panel,
    "p-button": Button,
    "p-input-text": InputText,
    Drawer,
    Divider
  },
  directives: {
    tooltip: Tooltip
  },
  data() {
    return {
      chats: [],
      editingChatId: null,
      authStore: null,
      route: null,
      isDesktop: window.innerWidth >= 768,
    };
  },
  beforeMount() {
    this.authStore = useAuthStore();
    this.route = useRoute();
    this.fetchChats();
    window.addEventListener("resize", this.updateScreenSize);
  },
  beforeUnmount() {
    window.removeEventListener("resize", this.updateScreenSize);
  },
  methods: {
    updateScreenSize() {
      this.isDesktop = window.innerWidth >= 768;
    },
    async fetchChats() {
      try {
        const response = await ApiService.getUserChats(this.authStore.User.userId);
        this.chats = response;
      } catch (error) {
        console.error("Error fetching chats:", error);
      }
    },
    async startNewChat() {
      try {
        const newChat = await this.authStore.createNewChat();
        this.chats = this.authStore.chats;
        this.$router.push(`/chats/${newChat.chatId}`);
      } catch (error) {
        console.error("Error creating a new chat:", error);
      }
    },
    editChat(chatId) {
      this.editingChatId = chatId;
    },
    async saveChat(chat) {
      try {
        this.editingChatId = null;
        await ApiService.updateChat(this.authStore.User.userId, {
          name: chat.name,
          chatId: chat.chatId,
        });
      } catch (error) {
        console.error("Error updating chat:", error);
      }
    },
    selectChat(chat) {
      if (chat.chatId !== this.route.params.chatGuid) {
        this.$router.push(`/chats/${chat.chatId}`);
      }
      this.authStore.isSidebarVisible = false;
    },
  },
};
</script>

<style scoped>
.custom-drawer :deep(.p-drawer-content) {
  background: var(--sidebar-bg, #202123);
  padding: 0;
}

.sidebar-panel {
  background: transparent;
  border: none;
  height: 100%;
}

.sidebar-panel :deep(.p-panel-header) {
  background: transparent;
  border: none;
  color: white;
  font-size: 1.2rem;
  font-weight: 600;
  padding: 1.5rem 1rem 0.5rem;
}

.sidebar-panel :deep(.p-panel-content) {
  background: transparent;
  border: none;
  padding: 0.5rem;
}

.new-chat {
  text-align: center;
  padding: 0.5rem 0 1rem;
}

.new-chat-button {
  width: 90%;
  transition: transform 0.2s ease;
}

.new-chat-button:hover {
  transform: scale(1.02);
}

.chat-list {
  list-style: none;
  padding: 0;
  margin: 0;
  max-height: calc(100vh - 35vh);
  overflow-y: auto;
}

.chat-list::-webkit-scrollbar {
  width: 6px;
}

.chat-list::-webkit-scrollbar-track {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 10px;
}

.chat-list::-webkit-scrollbar-thumb {
  background: rgba(255, 255, 255, 0.2);
  border-radius: 10px;
}

.chat-list::-webkit-scrollbar-thumb:hover {
  background: rgba(255, 255, 255, 0.3);
}

.chat-item {
  display: flex;
  align-items: center;
  padding: 0.75rem 0.5rem;
  margin-bottom: 0.25rem;
  border-radius: 8px;
  transition: all 0.2s ease;
  border: 1px solid transparent;
}

.chat-item:hover {
  background-color: var(--hover-bg, #343541);
}

.chat-item.active {
  background-color: var(--hover-bg, #343541);
  border-left: 3px solid var(--p-primary-color);
}

.chat-content {
  display: flex;
  align-items: center;
  width: 100%;
  position: relative;
}

.chat-icon {
  margin-right: 0.75rem;
  font-size: 1rem;
  opacity: 0.7;
}

.chat-title {
  cursor: pointer;
  display: flex;
  align-items: center;
  padding: 0.5rem;
  flex: 1;
  font-size: 0.95rem;
  color: white;
  border-radius: 6px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.chat-name {
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.chat-actions {
  opacity: 1;
}


.edit-button {
  color: rgba(255, 255, 255, 0.7);
  transition: color 0.2s ease;
}

.edit-button:hover {
  color: white;
}

.chat-input {
  flex: 1;
  background-color: var(--input-bg, #3a3a3a);
  border: none;
  color: white;
  padding: 0.5rem;
  border-radius: 6px;
  font-size: 0.95rem;
}

.no-chats {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem 1rem;
  color: rgba(255, 255, 255, 0.6);
}

.no-chat-icon {
  font-size: 2rem;
  margin-bottom: 1rem;
}

@media (min-width: 768px) {
  .sidebar-header {
    display: none;
  }
}
</style>