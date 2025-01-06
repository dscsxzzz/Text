<template>
    <div class="message-input-container">
      <p-inputtextarea
        v-model="message"
        rows="1"
        autoResize
        placeholder="Type a message..."
        class="message-input"
      />
      <p-button
        icon="pi pi-send"
        class="send-button"
        @click="sendMessage"
        :disabled="!message.trim()"
      />
    </div>
  </template>
  
  <script>
  import { Button, InputText } from "primevue";
  
  export default {
    components: {
      "p-inputtextarea": InputText,
      "p-button": Button,
    },
    data() {
      return {
        message: "",
      };
    },
    methods: {
      sendMessage() {
        if (this.message.trim()) {
          this.$emit("send", this.message);
          this.message = ""; // Clear input after sending
        }
      },
    },
  };
  </script>
  
  <style scoped>
  .message-input-container {
    display: flex;
    align-items: center;
    align-self: center;
    padding: 0.5rem;
    background-color: var(--input-bg, #343541);
    border-top: 1px solid var(--input-border, #555555);
    width: 40%;
  }
  
  .message-input {
    flex: 1;
    margin-right: 0.5rem;
    background-color: var(--textarea-bg, #404452);
    color: var(--text-color, #ffffff);
    border: none;
    border-radius: 8px;
    padding: 0.5rem 0.75rem;
    font-size: 1rem;
    outline: none;
  }
  
  .message-input::placeholder {
    color: var(--placeholder-color, #bbbbbb);
  }
  
  .send-button {
    background-color: var(--send-bg, #66ccff);
    color: var(--send-color, #ffffff);
    border: none;
    border-radius: 50%;
    width: 2.5rem;
    height: 2.5rem;
    display: flex;
    justify-content: center;
    align-items: center;
  }
  
  .send-button:disabled {
    background-color: var(--send-disabled-bg, #888888);
    color: var(--send-disabled-color, #cccccc);
  }
  </style>
  