<template>
    <div class="chat-window">
      <div class="messages" ref="messages">
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
      </div>
      <MessageInput @sendMessage="sendMessage" />
    </div>
  </template>
  
  <script>
  import { Card } from 'primevue';
import MessageInput from './MessageInput.vue';
  
  export default {
    components: {
      MessageInput,
      Card
    },
    data() {
      return {
        messages: [
          { id: 1, text: 'Hello! How can I help you?', type: 'bot' },
          { id: 2, text: 'Can you tell me about PrimeVue?', type: 'user' },
        ],
      };
    },
    methods: {
      sendMessage(text) {
        this.messages.push({ id: Date.now(), text, type: 'user' });
        // Simulate bot response
        setTimeout(() => {
          this.messages.push({
            id: Date.now(),
            text: 'PrimeVue is a UI library for Vue!',
            type: 'bot',
          });
        }, 1000);
      },
    },
  };
  </script>
  
  <style scoped>
  .chat-window {
    flex: 1;
    display: flex;
    flex-direction: column;
    background-color: var(--chat-bg, #343541);
    color: white;
  }
  
  .messages {
  flex: 1;
  padding: 1rem;
  overflow-y: auto;
  background-color: var(--chat-bg, #343541);
}

.message {
  display: flex;
  justify-content: flex-start; /* Default alignment for bot messages */
  margin-bottom: 1rem;
}

.message.user {
  justify-content: flex-end; /* Align user messages to the right */
}

.message-card {
  max-width: 60%; /* Prevent messages from stretching too wide */
  padding: 0.8rem;
  border-radius: 12px;
  word-wrap: break-word;
}

.message-card.bot {
  background-color: var(--bot-bg, #404452);
  color: var(--bot-text, #ffffff);
}

.message-card.user {
  background-color: var(--user-bg, #66ccff);
  color: var(--user-text, #ffffff);
}

  </style>
  