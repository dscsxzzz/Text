<template>
  <div class="chat-window">
    <div class="messages" ref="messages">
      <template v-if="messages.length">
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
              <p>{{ message.text }}</p>
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
import MessageInput from "./MessageInput.vue";
import * as signalR from "@microsoft/signalr";
import ApiService from "@/ApiService"; // Import your ApiService
import { useRoute } from "vue-router"; // To get the chatId from the route

export default {
  components: {
    MessageInput,
    Card,
  },
  data() {
    return {
      messages: [], // Holds chat messages
      hubConnection: null, // SignalR Hub connection
      userId: "unique-user-id", // Replace with actual user identifier
    };
  },
  methods: {
    async fetchChatMessages(chatId) {
      try {
        const chat = await ApiService.getUserChat(chatId); // Fetch chat details from the API
        this.messages = chat.Messages.sort((a, b) => new Date(a.createdAt) - new Date(b.createdAt)); // Sort messages by ID
      } catch (error) {
        console.error("Error fetching chat messages:", error);
      }
    },
    initializeSignalRConnection(chatId) {
      this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5000/chatHub") // Update with your SignalR Hub URL
        .build();

      // Event listener for receiving messages
      this.hubConnection.on("ReceiveMessage", (user, message) => {
        const messageType = user === this.userId ? "user" : "bot";
        this.messages.push({
          id: Date.now(),
          text: message,
          type: messageType,
        });
      });

      // Start the SignalR connection
      this.hubConnection
        .start()
        .then(() => {
          console.log("SignalR connection established.");
          // Join chat room or handle logic for this specific chatId if required
          this.hubConnection.invoke("JoinChat", chatId);
        })
        .catch((err) =>
          console.error("Error establishing SignalR connection:", err)
        );
    },
    sendMessage(text) {
      // if (!this.hubConnection) {
      //   console.error("SignalR connection not established.");
      //   return;
      // }

      // Push the user's message locally
      this.messages.push({
        id: Date.now(),
        text,
        type: "user",
      });

      this.messages.push({
        id: Date.now(),
        text,
        type: "bot",
      });

      this.scrollToBottom();

      // Send the message to the server
      // this.hubConnection
      //   .invoke("SendMessage", this.userId, text)
      //   .catch((err) => console.error("Error sending message:", err));
    },
    scrollToBottom() {
      this.$nextTick(() => {
        const messagesContainer = this.$refs.messages;
        messagesContainer.scrollTo({
          top: messagesContainer.scrollHeight,
          behavior: "smooth", // Smooth scrolling
        });
      });
    },
    cleanupSignalRConnection() {
      if (this.hubConnection) {
        this.hubConnection.stop().catch((err) =>
          console.error("Error disconnecting SignalR connection:", err)
        );
      }
    },
  },
  async mounted() {
    const route = useRoute(); // Access route object
    const chatGuid = route.params.chatGuid; // Extract chatId from the route params
    console.log(chatGuid);
    if (chatGuid) {
      await this.fetchChatMessages(chatGuid); // Fetch messages for the chat
      this.initializeSignalRConnection(chatGuid); // Start SignalR connection for this chat
    }
  },

  watch: {
    $route: {
      immediate: true, // Ensures this runs on the initial mount
      async handler(newRoute) {
        console.log(newRoute);
        const chatGuid = newRoute.params.chatGuid; // Extract new chatId
        console.log(chatGuid);
        if (chatGuid) {
          await this.fetchChatMessages(chatGuid); // Fetch messages for the new chat
          this.initializeSignalRConnection(chatGuid); // Start SignalR connection for the new chat
        }
      },
    },
  },
  beforeUnmount() {
    this.cleanupSignalRConnection(); // Cleanup SignalR connection when component is destroyed
  },
};
</script>

<style scoped>
.chat-window {
  flex: 1;
  display: flex;
  flex-direction: column;
  background-color: var(--chat-bg, #1f2937); /* Darker, modern background */
  color: white;
  height: 100%;
  border-radius: 12px; /* Rounded corners for the chat window */
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1); /* Soft shadow for better visual appeal */
  overflow: hidden; /* Prevent content overflow */
}

.messages {
  flex: 1;
  padding: 1rem;
  overflow-y: auto; /* Make the message area scrollable */
  background-color: var(--chat-bg, #1f2937); /* Consistent background */
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
  color: var(--bot-text, #ffffff);
  border: 1px solid #4b5563; /* Subtle border for bot messages */
}

.message-card.user {
  background-color: var(--user-bg, #2563eb);
  color: var(--user-text, #ffffff);
  border: 1px solid #1d4ed8; /* Subtle border for user messages */
}

.input-container {
  padding: 1rem;
  background-color: var(--input-bg, #111827);
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
  border-top: 1px solid #374151; /* Separator line for input area */
  box-shadow: 0 -2px 6px rgba(0, 0, 0, 0.1); /* Soft shadow at the bottom */
}
</style>
