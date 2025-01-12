<template>
    <div class="register-page">
      <Card class="p-shadow-6 register-container">
        <template #title>
          <h2 class="register-header-title">Create Account</h2>
          <p class="register-header-description">Please fill in the details to register</p>
        </template>
        <template #content>
          <Form
            v-slot="$form"
            :resolver="resolver"
            :initialValues="user"
            @submit="handleRegister"
            class="register-form"
          >
            <FloatLabel>
              <label for="username">Username</label>
              <InputText id="username" v-model="user.username" class="input-text" />
            </FloatLabel>
            <FloatLabel>
                <Password
                id="password"
                v-model="user.password"
                :feedback="feedback"
                toggleMask
                class="input-password"
                fluid
                
                />
                <label for="password">Password</label>
            </FloatLabel>
            <FloatLabel>
                <Password
                id="confirmPassword"
                v-model="user.confirmPassword"
                :feedback="feedback"
                toggleMask
                class="input-password"
                fluid
                />
                <label for="confirmPassword">Confirm Password</label>
            </FloatLabel>
            <div class="form-actions">
              <Button label="Register" class="btn-submit" type="submit" />
              <Button
                label="Back to Login"
                class="btn-back"
                type="button"
                @click="redirectToLogin"
              />
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
  
  export default {
    name: "RegisterPage",
    components: { InputText, Password, Button, Toast, Form, FloatLabel },
    setup() {
      const user = reactive({
        username: "",
        password: "",
        confirmPassword: "",
      });
      const feedback = false;
      const toast = ref(null);
      const authStore = useAuthStore();
  
      const handleRegister = async () => {
        if (!user.username || !user.password || !user.confirmPassword) {
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
            password: user.password,
          });
          console.log(registered);
          if (registered.StatusCode == 200) {
            toast.value.add({
              severity: "success",
              summary: "Registration Successful",
              detail: "You can now log in.",
            });
            setTimeout(() => {
              window.location.href = "/login"; // Redirect to login page after successful registration
            }, 1500);
          } else {
            toast.value.add({
              severity: "error",
              summary: "Registration Failed",
              detail: "Username already exists or another error occurred.",
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
  
      const redirectToLogin = () => {
        window.location.href = "/login"; // Redirect to login page
      };
  
      return { user, feedback, toast, handleRegister, redirectToLogin };
    },
  };
  </script>
  
  <style scoped>
  .register-page {
    display: flex;
    justify-content: center;
    align-items: center;
    min-height: 100vh;
    background: linear-gradient(135deg, #6a11cb, #2575fc);
    padding: 1rem;
    font-family: "Arial", sans-serif;
  }
  
  .register-container {
    width: 100%;
    max-width: 400px;
    background: #fff;
    border-radius: 8px;
    box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
    overflow: hidden;
  }
  
  .register-header-title {
    font-size: 1.8rem;
    color: #343541;
    margin-bottom: 0.5rem;
  }
  
  .register-header-description {
    color: #6c757d;
    font-size: 1rem;
  }
  
  .register-form {
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
  