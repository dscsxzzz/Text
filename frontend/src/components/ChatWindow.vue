<template>
   <Toast ref="toast" position="top-center" />
  <ConfirmDialog></ConfirmDialog>
  <div class="chat-window">
    <div class="messages" ref="messages">
      <div v-if="loadingChat">
        <ProgressSpinner />
      </div>
      <template v-if="messages.length && !loadingChat">
        <div
          v-for="message in messages"
          :key="message.id"
          :class="['message', message.type]"
        >
          <Card
            :class="['message-card', message.type]" 
            class="p-mb-3 p-shadow-2"
          >
            <template #content>
              <p>{{ message.messageText }}</p>
            </template>
          </Card>
        </div>
      </template>
      <div v-else class="empty-chat">
        <Card class="p-shadow-2">
          <template #content>
            <p class="empty-chat-message">Hello, how can I help you today?</p>
          </template>
        </Card>
      </div>
    </div>
    <div class="input-container">
      <MessageInput @sendMessage="sendMessage" />
    </div>
  </div>
</template>

<script>
import { Card } from "primevue";
import { useToast } from 'primevue/usetoast';
import MessageInput from "./MessageInput.vue";
import Toast from "primevue/toast";
import ConfirmDialog from "primevue/confirmdialog";
import * as signalR from "@microsoft/signalr";
import ApiService from "@/ApiService"; // Import your ApiService
import { useRoute } from "vue-router"; // To get the chatId from the route
import { useAuthStore } from "@/stores/authStore";
import { useConfirm } from "primevue/useconfirm";

export default {
  components: {
    MessageInput,
    ConfirmDialog,
    Card,
    Toast
  },
  props:{
    IsTryOut: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      messages: [], // Holds chat messages
      userHubConnection: null, // SignalR Hub connection for user messages
      botHubConnection: null,
      authStore: null,
      route: null,
      confirm: null,
      toast: null,
      loadingChat: true,
      tryOutMessageCount: 0,
      userId: null
    };
  },
  methods: {
    async fetchChatMessages(chatId) {
      console.log(!this.IsTryOut && chatId);
      if(!this.IsTryOut && chatId)
      {
        try {
          const chat = await ApiService.getUserChat(this.userId, chatId); // Fetch chat details from the API
          console.log(chat);
          this.messages = (chat.messages || []).sort(
            (a, b) => new Date(a.createdAt) - new Date(b.createdAt)
          );// Sort messages by ID
          this.scrollToBottom();
        } catch (error) {
          console.error("Error fetching chat messages:", error);
        }
      }
      this.loadingChat = false;
    },
    initializeSignalRConnections() {
      // Initialize bot message connection
      this.botHubConnection = new signalR.HubConnectionBuilder()
        .withUrl(`http://localhost:8080/senderhub?userId=${this.userId}`) 
        .configureLogging(signalR.LogLevel.Information) 
        // // Sender hub for bot messages
        .build();

      this.botHubConnection.on("ReceiveMessage", (message) => {
        console.log(message);
        const messageType = "bot";
        this.messages.push({
          id: Date.now(),
          messageText: message,
          type: messageType,
        });
        let messageCreateDto = {
          type: messageType,
          messageText: message
        };
        if(!this.IsTryOut)
        {
          const chatId = this.route.params.chatGuid;
          ApiService.postMessage(this.userId, chatId, messageCreateDto);
        }
        this.scrollToBottom();
      });

      this.botHubConnection
        .start()
        .then(() => {
          console.log("Bot SignalR connection established.");
        })
        .catch((err) =>
          console.error("Error establishing bot SignalR connection:", err)
        );

      // Initialize user message connection
      this.userHubConnection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:8081/receiverhub") // Receiver hub for user messages
        .build();

      this.userHubConnection
        .start()
        .then(() => {
          console.log("User SignalR connection established.");
        })
        .catch((err) =>
          console.error("Error establishing user SignalR connection:", err)
        );
    },
    sendMessage(text) {
      if (!this.userHubConnection) {
        console.error("SignalR connection for user messages not established.");
        return;
      }

      // Push the user's message locally
      this.messages.push({
        id: Date.now(),
        messageText: text,
        type: "user",
      });
      if(!this.IsTryOut)
      {
        let messageCreateDto = {
          type: "user",
          messageText: text
        };
  
        const chatId = this.route.params.chatGuid;
        ApiService.postMessage(this.authStore.user.userId, chatId, messageCreateDto);
      }
      else{
        this.tryOutMessageCount += 1;

        if(this.tryOutMessageCount == 4)
        {
          setTimeout(() => {
                this.confirm.require({
                    message: 'Looks like you enjoy our service. Do you want to register an account to unlock full potential?',
                    header: 'Unlock Full Potential',
                    icon: 'pi pi-star',
                    rejectProps: {
                        label: 'Keep Using',
                        severity: 'secondary',
                        outlined: true
                    },
                    acceptProps: {
                        label: 'Register',
                        severity: 'success'
                    },
                    accept: (event) => {
                        event?.stopPropagation(); // Prevents further key event issues
                        this.toast.add({
                            severity: 'success',
                            summary: 'Registered',
                            detail: 'You have registered successfully!',
                            life: 3000
                        });
                        setTimeout(() => {
                            window.location.href = "/register"; // Redirect only if user accepts
                        }, 1000);
                    },
                    reject: (event) => {
                        event?.stopPropagation();
                        this.toast.add({
                            severity: 'info',
                            summary: 'Continue as Guest',
                            detail: 'You chose to keep using without registration',
                            life: 3000
                        });
                    }
                });
            }, 100); 
        }
      }

      this.scrollToBottom();
      

      this.userHubConnection
        .invoke("HandleFrontendRequest", this.userId, text)
        .catch((err) =>
          console.error("Error sending message to user SignalR connection:", err)
        );
    },
    scrollToBottom() {
      this.$nextTick(() => {
        const messagesContainer = this.$refs.messages;
        if (messagesContainer) {
          messagesContainer.scrollTo({
            top: messagesContainer.scrollHeight,
            behavior: "smooth", // Smooth scrolling
          });
        }
      });
    },
    cleanupSignalRConnections() {
      // Cleanup bot hub connection
      if (this.botHubConnection) {
        this.botHubConnection
          .stop()
          .catch((err) =>
            console.error("Error disconnecting bot SignalR connection:", err)
          );
      }

      // Cleanup user hub connection
      if (this.userHubConnection) {
        this.userHubConnection
          .stop()
          .catch((err) =>
            console.error("Error disconnecting user SignalR connection:", err)
          );
      }
    },
  },
  async mounted() {
    this.authStore = useAuthStore();
    this.confirm = useConfirm();
    this.toast = useToast();
    this.userId = this.IsTryOut ? crypto.randomUUID() : this.authStore.user.userId;
    console.log("mounted", this.authStore);
    this.route = useRoute(); // Access route object
    const chatGuid = this.route.params.chatGuid; // Extract chatId from the route params
    await this.fetchChatMessages(chatGuid); // Fetch messages for the chat
    this.initializeSignalRConnections(); // Start SignalR connections for this chat
  },
  watch: {
    $route: {
      immediate: true, // Ensures this runs on the initial mount
      async handler(newRoute) {
        const chatGuid = newRoute.params.chatGuid; // Extract new chatId
        if (chatGuid) {
          console.log("Switching to new chat:", chatGuid);
          await this.fetchChatMessages(chatGuid); // Fetch messages for the new chat
        }
      },
    },
  },
  beforeUnmount() {
    this.cleanupSignalRConnections(); // Cleanup SignalR connections when component is destroyed
  }
};
</script>


<style scoped>
.chat-window {
  flex: 1;
  display: flex;
  flex-direction: column;
  background: var(--surface-ground);
  color: white;
  height: 100%;
  overflow: hidden; /* Prevent content overflow */
}

.messages {
  flex: 1;
  padding: 1rem;
  overflow-y: auto; /* Make the message area scrollable */
  background: var(--surface-ground);
  /* Consistent background */
  display: flex;
  flex-direction: column;
  gap: 1rem; /* Space between messages */
}

.messages::-webkit-scrollbar {
  width: 8px; /* Narrow scrollbar for a sleeker look */
}

.messages::-webkit-scrollbar-track {
  background: #2a2b32; /* Track color */
}

.messages::-webkit-scrollbar-thumb {
  background: #4b5563; /* Thumb color */
  border-radius: 4px; /* Rounded scrollbar for modern feel */
}

.empty-chat {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
}

.empty-chat-message {
  font-size: 1.2rem;
  color: var(--text-color, #ffffff);
  text-align: center;
  padding: 1rem;
}

.message {
  display: flex;
  margin-bottom: 1rem;
  width: 100%;
}

.message.bot {
  justify-content: flex-start; /* Default alignment for bot messages */
}

.message.user {
  justify-content: flex-end; /* Align user messages to the right */
}

.message-card {
  max-width: 70%; /* Prevent messages from stretching too wide */
  padding: 1rem; /* Larger padding for better spacing */
  border-radius: 12px;
  word-wrap: break-word;
  font-size: 1rem; /* Improved readability */
  line-height: 1.5; /* Better text spacing */
}

.message-card.bot {
  background-color: var(--bot-bg, #374151);
  color: var(--p-primary-color);
  border: 1px solid #4b5563; /* Subtle border for bot messages */
}

.message-card.user {
  color: var(--user-text, #ffffff);
}

.input-container {
  padding: 1rem;
  background: var(--surface-ground);
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
}
</style>
