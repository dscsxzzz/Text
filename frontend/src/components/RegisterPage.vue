<template>
  <div class="register-page">
    <Card class="p-shadow-6 register-container">
      <template #title>
        <h2 class="register-header-title">Create Account</h2>
        <p class="register-header-description">Please fill in the details to register</p>
      </template>
      <template #content>
        <Form v-if="!isConfirming" v-slot="$form" :resolver="resolver" :initialValues="user" @submit="handleRegister" class="register-form">
          <FloatLabel>
            <label for="username">Username</label>
            <InputText id="username" v-model="user.username" class="input-text" />
          </FloatLabel>
          <FloatLabel>
            <InputText id="email" v-model="user.email" class="input-text" />
            <label for="email">E-Mail</label>
          </FloatLabel>
          <FloatLabel>
            <Password id="password" v-model="user.password" :feedback="feedback" toggleMask class="input-password" fluid />
            <label for="password">Password</label>
          </FloatLabel>
          <FloatLabel>
            <Password id="confirmPassword" v-model="user.confirmPassword" :feedback="feedback" toggleMask class="input-password" fluid />
            <label for="confirmPassword">Confirm Password</label>
          </FloatLabel>
          <div class="form-actions">
            <Button label="Register" class="btn-submit" type="submit" />
            <Button label="Back to Login" class="btn-back" type="button" @click="redirectToLogin" />
          </div>
        </Form>

        <!-- Email Confirmation Form -->
        <div v-else class="confirmation-form">
          <h3>Confirm Your Email</h3>
          <p>Please enter the confirmation code sent to your email.</p>
          <FloatLabel>
            <InputText id="confirmationCode" v-model="confirmationCode" class="input-text" />
            <label for="confirmationCode">Confirmation Code</label>
          </FloatLabel>
          <div class="form-actions">
            <Button label="Confirm" class="btn-submit" @click="handleConfirmEmail" />
          </div>
        </div>

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

export default {
  name: "RegisterPage",
  components: { InputText, Password, Button, Toast, Form, FloatLabel },
  setup() {
    const user = reactive({
      username: "",
      password: "",
      confirmPassword: "",
      email: "",
    });
    const confirmationCode = ref("");
    const feedback = false;
    const toast = ref(null);
    const authStore = useAuthStore();
    const isConfirming = ref(false); // Flag to toggle confirmation step

    const handleRegister = async () => {
      if (!user.username || !user.password || !user.confirmPassword || !user.email) {
        toast.value.add({
          severity: "warn",
          summary: "Missing Information",
          detail: "Please fill in all fields.",
        });
        return;
      }
      if (user.password !== user.confirmPassword) {
        toast.value.add({
          severity: "error",
          summary: "Password Mismatch",
          detail: "Your passwords do not match.",
        });
        return;
      }
      try {
        let registered = await authStore.register({
          username: user.username,
          email: user.email,
          password: user.password,
        });

        if (registered.status === 200) {
          isConfirming.value = true; // Switch to email confirmation step
          toast.value.add({
            severity: "success",
            summary: "Registration Successful",
            detail: "A confirmation code has been sent to your email.",
          });
        } else {
          const data = registered.response.data;
          let errorMessages = "";
          if(data.errors)
          {
            errorMessages = Object.keys(data.errors)
              .map((key) => `${key}: ${data.errors[key].join(", ")}`)
              .join("\n");
          }
          else{
            errorMessages = data;
          }

          toast.value.add({
            severity: "error",
            summary: "Registration Failed",
            detail: errorMessages,
          });
        }
      } catch (error) {
        toast.value.add({
          severity: "error",
          summary: "Registration Failed",
          detail: "An error occurred while registering.",
        });
      }
    };

    const handleConfirmEmail = async () => {
      try {
        let response = await authStore.confirmEmail(confirmationCode.value);

        if (response.status === 200) {
          toast.value.add({
            severity: "success",
            summary: "Email Confirmed",
            detail: "Your email has been verified. Redirecting to login...",
          });
          setTimeout(() => {
            window.location.href = "/login";
          }, 1500);
        } else {
          toast.value.add({
            severity: "error",
            summary: "Confirmation Failed",
            detail: "Invalid confirmation code. Please try again.",
          });
        }
      } catch (error) {
        toast.value.add({
          severity: "error",
          summary: "Error",
          detail: "An error occurred while confirming your email.",
        });
      }
    };

    const redirectToLogin = () => {
      window.location.href = "/login";
    };

    return { user, feedback, toast, handleRegister, redirectToLogin, isConfirming, confirmationCode, handleConfirmEmail };
  },
};
</script>

<style scoped>
.register-page {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: var(--surface-ground);
  padding: 1rem;
  font-family: "Arial", sans-serif;
}

.register-container {
  width: 100%;
  max-width: 400px;
  border-radius: 8px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
  overflow: hidden;
}

.register-header-title {
  font-size: 1.8rem;
  color: var(--text-color);
  margin-bottom: 0.5rem;
}

.register-header-description {
  color: var(--text-color);
  font-size: 1rem;
}

.register-form,
.confirmation-form {
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
