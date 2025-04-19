<template>
    <div class="main-layout">
      <Sidebar />
      <div class="chat-container">
        <Header />
        <ChatWindow v-bind:IsTryOut="false"/>
      </div>
    </div>
  </template>
  
  <script>
  import Header from './Header.vue';
  import Sidebar from './Sidebar.vue';
  import ChatWindow from './ChatWindow.vue';
  import { useAuthStore } from '@/stores/authStore'


  export default {
    components: {
      Header,
      Sidebar,
      ChatWindow,
    },
    beforeMount: (() => {
      const auth = useAuthStore()
      auth.checkExpiration()
    })
  };
  </script>
  
  <style scoped>
  .main-layout {
    display: flex;
    flex-direction: row;
    height: 100vh;
    width: 100vw;
  }
  
  .chat-container {
    display: flex;
    flex-direction: column;
    flex: 1;
    overflow: hidden;
  }
  </style>
  