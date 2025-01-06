<template>
    <div class="sidebar">
      <p-button label="New Chat" class="mb-3" @click="startNewChat" />
      <ul class="chat-list">
        <li v-for="chat in chats" :key="chat.id" @click="selectChat(chat)">
          <p>{{ chat.title }}</p>
        </li>
      </ul>
    </div>
  </template>
  
  <script>
  import { Button } from 'primevue';
  
  export default {
    components: {
      'p-button': Button,
    },
    data() {
      return {
        chats: [
          { id: 1, title: 'Chat 1' },
          { id: 2, title: 'Chat 2' },
        ],
      };
    },
    methods: {
      startNewChat() {
        console.log('Start new chat');
      },
      selectChat(chat) {
        console.log('Chat selected:', chat);
      },
    },
  };
  </script>
  
  <style scoped>
  .sidebar {
    background-color: var(--sidebar-bg, #202123);
    color: white;
    width: 250px;
    padding: 1rem;
    overflow-y: auto;
  }
  .chat-list {
    list-style: none;
    padding: 0;
  }
  .chat-list li {
    cursor: pointer;
    padding: 0.5rem;
    border-radius: 5px;
    margin-bottom: 0.5rem;
    transition: background-color 0.3s;
  }
  .chat-list li:hover {
    background-color: var(--hover-bg, #343541);
  }
  </style>
  