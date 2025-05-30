<template>
  <div class="login-page">
    <Card class="p-shadow-6 login-container">
      <template #title>
        <h2 class="login-header-title">Welcome Back</h2>
        <p class="login-header-description">Please log in to continue</p>
      </template>
      <template #content>
        <Form
          v-slot="$form"
          :resolver="resolver"
          :initialValues="user"
          @submit="handleLogin"
          class="login-form"
        >
          <FloatLabel>
            <label for="username">Username</label>
            <InputText id="username" v-model="user.username" class="input-width" />
          </FloatLabel>
          <FloatLabel>
            <Password
              inputId="password_input"
              v-model="user.password"
              :feedback="feedback"
              class="input-width"
              toggle-mask
              fluid
            />
            <label for="password_input">Password</label>
          </FloatLabel>
          
          <!-- Forgot Password Button -->
          <div class="forgot-password-container">
            <a @click="redirectToForgotPassword" class="forgot-password-link">Forgot Password?</a>
          </div>
          
          <div class="form-actions">
            <Button
              label="Register"
              class="btn-register"
              type="button"
              @click="redirectToRegister"
            />
            <Button label="Login" class="btn-submit" type="submit" />
          </div>
        </Form>
        <Toast ref="toast" position="top-center" />
      </template>
    </Card>
    
  </div>
</template>

<script>
import { ref, reactive } from "vue";
import InputText from "primevue/inputtext";
import Password from "primevue/password";
import Button from "primevue/button";
import Toast from "primevue/toast";
import { FloatLabel } from "primevue";
import { useAuthStore } from "@/stores/authStore";
import { Form } from "@primevue/forms";
import router from "@/router";

export default {
  name: "LoginPage",
  components: { InputText, Password, Button, Toast, Form, FloatLabel},
  setup() {
    const user = reactive({
      username: "",
      password: "",
    });
    const feedback = false;
    const toast = ref(null);
    const authStore = useAuthStore();

    const handleLogin = async () => {
      if (!user.username || !user.password) {
        toast.value.add({
          severity: "warn",
          summary: "Missing Information",
          detail: "Please fill in both username and password.",
        });
        return;
      }
      try {
        let loggedIn = await authStore.login({
          username: user.username,
          password: user.password,
        });
        if (loggedIn) {
          toast.value.add({
            severity: "success",
            summary: "Login Successful",
            detail: "Redirecting to the chat...",
          });
          setTimeout(() => {
            router.push("/chats/");
          }, 1500);
        } else {
          toast.value.add({
            severity: "error",
            summary: "Login Failed",
            detail: "Invalid username or password.",
          });
        }
      } catch (error) {
        toast.value.add({
          severity: "error",
          summary: "Login Failed",
          detail: "Invalid username or password.",
        });
      }
    };

    const redirectToRegister = () => {
      window.location.href = "/register";
    };

    const redirectToForgotPassword = () => {
      window.location.href = "/forgot-password";
    }

    return { user, feedback, toast, handleLogin, redirectToRegister, redirectToForgotPassword};
  },
};
</script>

<style scoped>
.login-page {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: var(--surface-ground);
  padding: 1rem;
  font-family: "Arial", sans-serif;
}

.login-header-title {
  font-size: 1.8rem;
  color: var(--text-color);
  margin-bottom: 0.5rem;
}
  
.login-header-description {
  color: var(--text-color);
  font-size: 1rem;
}

.login-container {
  width: 100%;
  max-width: 400px;
  border-radius: 8px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
  overflow: hidden;
}

.forgot-password-container {
  display: flex;
  justify-content: flex-end;
  margin-top: -10px;
}

.forgot-password-link {
  font-size: 0.9rem;
  color: #007bff;
  cursor: pointer;
  text-decoration: underline;
}

.forgot-password-link:hover {
  color: #0056b3;
}

.login-form {
  display: flex;
  flex-direction: column;
  margin-top: 30px;
  gap: 1.5rem;
}

.form-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>
