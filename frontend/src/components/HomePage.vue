<template>
    <div class="home-page">
      <!-- Hero Section -->
      <div class="hero">
        <div class="hero-text">
          <h1>Welcome to AI Chat Summarizer</h1>
          <p>
            Experience the power of AI with our intelligent chat assistant, built
            to summarize conversations, extract insights, and enhance
            productivity.
          </p>
          <p-button label="Get Started" class="hero-button" @click="goToChat" />
        </div>
        <img
          src="https://via.placeholder.com/600x400"
          alt="AI Chat Illustration"
          class="hero-image"
        />
      </div>
  
      <!-- Features Section -->
      <div class="features">
        <h2 class="section-title">Why Choose Our App?</h2>
        <Timeline :value="features">
            <template #opposite="slotProps">
                {{ slotProps.item.title }}
            </template>
            <template #content="slotProps">
                {{ slotProps.item.description }}
            </template>
        </Timeline>
      </div>
  
      <!-- Demo Section -->
      <div class="demo-section">
        <h2 class="section-title">See It in Action</h2>
        <div class="demo-content">
          <img
            src="https://via.placeholder.com/500x300"
            alt="Demo Illustration"
            class="demo-image"
          />
          <div class="demo-text">
            <p>
              Start a chat, or interact with our AI to generate
              insightful summaries of your conversations.
            </p>
            <p-button label="Try Demo" class="demo-button" @click="goToTryOut"/>
          </div>
        </div>
      </div>
  
      <!-- Footer -->
      <footer class="footer">
        <p>© 2025 AI Chat Summarizer. All Rights Reserved.</p>
      </footer>
    </div>
  </template>
  
  <script>
  import { Button } from "primevue";
  import { Timeline } from "primevue";
  import { useRouter } from "vue-router"; // Importing useRouter for routing

export default {
  components: {
    "p-button": Button,
    "p-timeline": Timeline,
  },
  setup() {
    const router = useRouter(); // Hook to get access to router

    const features = [
      {
        title: "Summarize Conversations",
        description:
          "Get concise and meaningful summaries of your chats instantly.",
      },
      {
        title: "AI-Powered Insights",
        description:
          "Leverage AI to extract key points and actionable insights.",
      },
      {
        title: "Real-Time Assistance",
        description:
          "Interact with the AI assistant for quick and accurate responses.",
      },
      {
        title: "Seamless Integration",
        description:
          "Easily integrate with your favorite tools and workflows.",
      },
    ];
    const goToTryOut = () => {
      router.push("/try-out");
    };

    // Logic for navigating to /chat on button click
    const goToChat = () => {
      router.push("/chats"); // Redirects to /chat page
    };


    return {
      features,
      goToChat,
      goToTryOut
    };
  },
};
</script>
  <style scoped>
  .home-page {
    font-family: "Arial", sans-serif;
    color: #333;
  }
  
  /* Hero Section */
  .hero {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 2rem;
    background-color: #f9fafb;
    animation: fadeIn 1s ease-in-out;
  }
  
  .hero-text {
    max-width: 50%;
  }
  
  .hero-text h1 {
    font-size: 2.5rem;
    margin-bottom: 1rem;
  }
  
  .hero-text p {
    margin-bottom: 1rem;
    font-size: 1.2rem;
  }
  
  .hero-button {
    background-color: #007bff;
    color: white;
    border: none;
  }
  
  .hero-image {
    max-width: 40%;
    border-radius: 1rem;
    animation: slideInRight 1s ease-in-out;
  }
  
  /* Features Section */
  .features {
    padding: 2rem;
    background-color: #ffffff;
    text-align: center;
  }
  
  .section-title {
    font-size: 2rem;
    margin-bottom: 1rem;
  }
  
  .features-timeline {
    margin-top: 2rem;
  }
  
  /* Demo Section */
  .demo-section {
    display: flex;
    flex-direction: column;
    align-items: center;
    padding: 2rem;
    background-color: #f9fafb;
  }
  
  .demo-content {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 2rem;
    margin-top: 1rem;
  }
  
  .demo-image {
    max-width: 50%;
    border-radius: 1rem;
  }
  
  .demo-text {
    max-width: 40%;
  }
  
  .demo-button {
    margin-top: 1rem;
    background-color: #28a745;
    color: white;
    border: none;
  }
  
  /* Footer */
  .footer {
    text-align: center;
    padding: 1rem;
    background-color: #343a40;
    color: white;
  }
  
  /* Animations */
  @keyframes fadeIn {
    from {
      opacity: 0;
    }
    to {
      opacity: 1;
    }
  }
  
  @keyframes slideInRight {
    from {
      transform: translateX(100%);
    }
    to {
      transform: translateX(0);
    }
  }
  </style>
  