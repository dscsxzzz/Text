<template>
    <div class="main-layout">
      <Header />
      <div class="chat-container">
        <Sidebar />
        <ChatWindow v-bind:IsTryOut="false"/>
      </div>
    </div>
  </template>
  
  <script>
  import Header from './Header.vue';
  import Sidebar from './Sidebar.vue';
  import ChatWindow from './ChatWindow.vue';
  
  export default {
    components: {
      Header,
      Sidebar,
      ChatWindow,
    },
  };
  </script>
  
  <style scoped>
  .main-layout {
    display: flex;
    flex-direction: column;
    height: 100vh;
    width: 100vw;
  }
  
  .chat-container {
    display: flex;
    flex: 1;
    overflow: hidden;
  }
  </style>
  