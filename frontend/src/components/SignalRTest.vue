<template>
    <div>
      <h1>SignalR Demo</h1>
      <input v-model="inputData" placeholder="Enter your input data" />
      <button @click="sendRequest">Send Request to Server</button>
      <div v-if="receivedMessages.length">
        <h2>Received Messages</h2>
        <ul>
          <li v-for="(message, index) in receivedMessages" :key="index">{{ message }}</li>
        </ul>
      </div>
    </div>
  </template>
  
  <script>
  import * as signalR from "@microsoft/signalr";
  
  export default {
    data() {
      return {
        inputData: "",
        receiverConnection: null, // Connection to FrontendReceiverHub
        senderConnection: null, // Connection to FrontendSenderHub
        receivedMessages: [],
        userId: "3fa85f64-5717-4562-b3fc-2c963f66afa6", // Replace with dynamic User ID if needed
      };
    },
    methods: {
      startReceiverConnection() {
        this.receiverConnection = new signalR.HubConnectionBuilder()
          .withUrl(`http://localhost:8081/receiverhub`)
          .build();
  
        this.receiverConnection
          .start()
          .then(() => console.log("Connected to FrontendReceiverHub"))
          .catch((err) =>
            console.error("Error connecting to FrontendReceiverHub:", err)
          );
      },
      startSenderConnection() {
        this.senderConnection = new signalR.HubConnectionBuilder()
          .withUrl(`http://localhost:8080/senderhub?userId=${this.userId}`)
          .build();
  
        this.senderConnection.on("ReceiveMessage", (message) => {
          console.log("Received processed message:", message);
          this.receivedMessages.push(message); // Add message to the list
        });
  
        this.senderConnection
          .start()
          .then(() => console.log("Connected to FrontendSenderHub"))
          .catch((err) =>
            console.error("Error connecting to FrontendSenderHub:", err)
          );
      },
      sendRequest() {
        if (this.receiverConnection) {
          this.receiverConnection
            .invoke("ReceiveRequestFromFrontend", this.userId, this.inputData)
            .then(() => console.log("Request sent to server."))
            .catch((err) => console.error("Error sending request:", err));
        } else {
          console.error("Receiver connection not established.");
        }
      },
      stopConnections() {
        if (this.receiverConnection) {
          this.receiverConnection.stop().catch((err) =>
            console.error("Error disconnecting ReceiverHub:", err)
          );
        }
        if (this.senderConnection) {
          this.senderConnection.stop().catch((err) =>
            console.error("Error disconnecting SenderHub:", err)
          );
        }
      },
    },
    mounted() {
      this.startReceiverConnection(); // Start the connection for sending requests
      this.startSenderConnection(); // Start the connection for receiving processed messages
    },
    beforeUnmount() {
      this.stopConnections(); // Cleanup connections when the component is destroyed
    },
  };
  </script>
  
  <style scoped>
  h1 {
    color: #333;
  }
  ul {
    list-style-type: none;
    padding: 0;
  }
  li {
    padding: 8px 0;
    border-bottom: 1px solid #ccc;
  }
  button {
    margin-top: 20px;
    padding: 10px 20px;
    background-color: #007bff;
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
  }
  button:hover {
    background-color: #0056b3;
  }
  </style>
  