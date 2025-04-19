<template>
  <div class="p-m-4">
    <!-- Hero Section -->
    <div class="p-d-flex p-flex-column p-md-flex-row p-ai-center p-jc-between surface-0 p-5 border-round shadow-2 mb-6 gap-4">
      <div class="p-flex-1">
        <h1 class="text-4xl font-bold mb-3">Welcome to AI Chat Summarizer</h1>
        <p class="text-lg mb-4">
          Experience the power of AI with our intelligent chat assistant. Instantly summarize conversations, extract key insights, and enhance productivity.
        </p>
        <p-button label="Get Started" icon="pi pi-arrow-right" @click="goToChat" />
      </div>
      <img
        src="https://via.placeholder.com/600x400"
        alt="AI Chat Illustration"
        class="border-round shadow-3"
        style="max-width: 100%; height: auto; max-width: 500px;"
      />
    </div>

    <!-- Features Section -->
    <div class="surface-ground border-round p-4 shadow-1 mb-6">
      <h2 class="text-center text-3xl font-medium mb-5">Why Choose Our App?</h2>
      <p-timeline :value="features" align="alternate">
        <template #opposite="slotProps">
          <span class="text-primary font-bold">{{ slotProps.item.title }}</span>
        </template>
        <template #content="slotProps">
          <div class="p-card p-p-3">
            <p>{{ slotProps.item.description }}</p>
          </div>
        </template>
      </p-timeline>
    </div>

    <!-- Demo Section -->
    <div class="surface-section border-round p-5 shadow-1 mb-6">
      <h2 class="text-2xl text-center mb-4">See It in Action</h2>
      <div class="p-d-flex p-flex-column p-md-flex-row p-ai-center p-jc-between gap-5">
        <img
          src="https://via.placeholder.com/500x300"
          alt="Demo Illustration"
          class="border-round shadow-2"
          style="max-width: 100%; height: auto; max-width: 400px;"
        />
        <div class="p-flex-1">
          <p class="mb-3">
            Start a chat or interact with our AI to generate insightful summaries of your conversations.
          </p>
          <p-button label="Try Demo" icon="pi pi-play" class="p-button-success" @click="goToTryOut" />
        </div>
      </div>
    </div>

    <!-- Footer -->
    <footer class="text-white text-center p-3 border-round-top-2">
      <p class="m-0">© 2025 AI Chat Summarizer. All Rights Reserved.</p>
    </footer>
  </div>
</template>

<script>
import {Button} from 'primevue';
import {Timeline} from 'primevue';
import { useRouter } from 'vue-router';

export default {
  components: {
    'p-button': Button,
    'p-timeline': Timeline,
  },
  setup() {
    const router = useRouter();

    const features = [
      {
        title: "Summarize Conversations",
        description: "Get concise and meaningful summaries of your chats instantly.",
      },
      {
        title: "AI-Powered Insights",
        description: "Leverage AI to extract key points and actionable insights.",
      },
      {
        title: "Real-Time Assistance",
        description: "Interact with the AI assistant for quick and accurate responses.",
      },
      {
        title: "Seamless Integration",
        description: "Easily integrate with your favorite tools and workflows.",
      },
    ];

    const goToTryOut = () => router.push("/try-out");
    const goToChat = () => router.push("/chats");

    return {
      features,
      goToTryOut,
      goToChat,
    };
  },
};
</script>

<style scoped>
.text-primary {
  color: var(--primary-color);
}
</style>
