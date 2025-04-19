<template>
  <div class="forgot-password-page">
    <Card class="forgot-password-card">
      <template #header>
        <div class="header-content">
          <h2 class="forgot-password-header-title">Reset Your Password</h2>
          <p class="forgot-password-header-description">Follow the steps below to reset your password</p>
        </div>
      </template>
      <template #content>
        <Toast ref="toast" position="top-right" />
        <Stepper value="1" linear>
          <StepList>
            <Step value="1">Enter Username</Step>
            <Step value="2">Enter Confirmation Code</Step>
            <Step value="3">Reset Password</Step>
          </StepList>
          <StepPanels>
            <!-- Step 1: Enter Username -->
            <StepPanel v-slot="{ activateCallback }" value="1">
              <div class="step-panel-content">
                <div class="form-container">
                  <Form @submit="handleForgotPassword">
                    <div class="p-fluid">
                      <FloatLabel>
                        <label for="forgot-username">Username</label>
                        <InputText 
                          id="forgot-username" 
                          v-model="forgotUsername" 
                          aria-describedby="username-help" 
                        />
                      </FloatLabel>
                      <small id="username-help" class="form-text">
                        Enter the username associated with your account.
                      </small>
                    </div>
                    <Button 
                      label="Send Reset Code" 
                      icon="pi pi-envelope" 
                      type="submit" 
                      class="mt-3 w-full"
                    />
                  </Form>
                </div>
                <div class="navigation-container">
                  <Button 
                    label="Next" 
                    icon="pi pi-arrow-right" 
                    iconPos="right" 
                    @click="activateCallback('2')" 
                    :disabled="!sentCode"
                  />
                </div>
              </div>
            </StepPanel>

            <!-- Step 2: Enter Confirmation Code -->
            <StepPanel v-slot="{ activateCallback }" value="2">
              <div class="step-panel-content">
                <div class="form-container">
                  <Form @submit="handleConfirmationCode">
                    <div class="p-fluid">
                      <FloatLabel>
                        <label for="confirmation-code">Confirmation Code</label>
                        <InputText 
                          id="confirmation-code" 
                          v-model="confirmationCode" 
                          aria-describedby="code-help" 
                        />
                      </FloatLabel>
                      <small id="code-help" class="form-text">
                        Enter the 6-digit code sent to your email address.
                      </small>
                    </div>
                    <Button 
                      label="Verify Code" 
                      icon="pi pi-check" 
                      type="submit" 
                      class="mt-3 w-full"
                    />
                  </Form>
                </div>
                <div class="navigation-container">
                  <Button 
                    label="Back" 
                    icon="pi pi-arrow-left" 
                    severity="secondary" 
                    @click="activateCallback('1')" 
                  />
                  <Button 
                    label="Next" 
                    icon="pi pi-arrow-right" 
                    iconPos="right" 
                    @click="activateCallback('3')" 
                    :disabled="!isCodeVerified"
                  />
                </div>
              </div>
            </StepPanel>

            <!-- Step 3: Enter New Password -->
            <StepPanel v-slot="{ activateCallback }" value="3">
              <div class="step-panel-content">
                <div class="form-container">
                  <Form @submit="handleNewPassword">
                    <div class="p-fluid">
                      <FloatLabel>
                        <label for="new-password">New Password</label>
                        <Password 
                          id="new-password" 
                          v-model="newPassword" 
                          :feedback="true"
                          aria-describedby="password-help"
                          class="input-password"
                          toggleMask 
                        />
                      </FloatLabel>
                      <small id="password-help" class="form-text">
                        Create a strong password with at least 8 characters.
                      </small>
                    </div>
                    <Button 
                      label="Reset Password" 
                      icon="pi pi-lock" 
                      type="submit" 
                      class="mt-3 w-full"
                    />
                  </Form>
                </div>
                <div class="navigation-container">
                  <Button 
                    label="Back" 
                    icon="pi pi-arrow-left" 
                    severity="secondary" 
                    @click="activateCallback('2')" 
                  />
                </div>
              </div>
            </StepPanel>
          </StepPanels>
        </Stepper>
      </template>
    </Card>
  </div>
</template>

<script>
import { ref } from "vue";
import Card from "primevue/card";
import InputText from "primevue/inputtext";
import Password from "primevue/password";
import Button from "primevue/button";
import Toast from "primevue/toast";
import FloatLabel from 'primevue/floatlabel';
import {Form} from "@primevue/forms";
import Stepper from 'primevue/stepper';
import StepList from 'primevue/steplist';
import StepPanels from 'primevue/steppanels';
import Step from 'primevue/step';
import StepPanel from 'primevue/steppanel';
import { useAuthStore } from "@/stores/authStore";
import { useToast } from "primevue/usetoast";
export default {
  name: "ForgotPasswordPage",
  components: { 
    Card, 
    InputText, 
    Password,
    Button, 
    Toast, 
    Form, 
    FloatLabel,
    Stepper,
    StepList,
    Step,
    StepPanels,
    StepPanel
  },
  setup() {
    const forgotUsername = ref("");
    const confirmationCode = ref("");
    const newPassword = ref("");
    const activeStep = ref("1");
    const toast = useToast();
    const authStore = useAuthStore();
    const sentCode = ref(false);
    const isCodeVerified = ref(false);


    const handleForgotPassword = async () => {
      if (!forgotUsername.value) {
        toast.add({
          severity: "warn",
          summary: "Missing Username",
          detail: "Please enter your username.",
          life: 3000
        });
        return;
      }
      try {
        // Send forgot password request
        await authStore.forgotPassword({ username: forgotUsername.value });
        toast.add({
          severity: "success",
          summary: "Reset Code Sent",
          detail: "Check your email for the reset code.",
          life: 3000
        });
        // Automatically move to next step
        sentCode.value = true;
      } catch (error) {
        toast.add({
          severity: "error",
          summary: "Error",
          detail: "Failed to send reset code. Please try again.",
          life: 3000
        });
      }
    };

    const handleConfirmationCode = async () => {
      if (!confirmationCode.value) {
        toast.add({
          severity: "warn",
          summary: "Missing Code",
          detail: "Please enter the confirmation code.",
          life: 3000
        });
        return;
      }
      try {
        // Verify the confirmation code
        await authStore.verifyResetCode({ 
          code: confirmationCode.value 
        });
        toast.add({
          severity: "success",
          summary: "Code Verified",
          detail: "The code has been verified. Now, enter your new password.",
          life: 3000
        });
        // Automatically move to next step
        isCodeVerified.value = true;
      } catch (error) {
        toast.add({
          severity: "error",
          summary: "Invalid Code",
          detail: "The confirmation code is incorrect or has expired.",
          life: 3000
        });
      }
    };

    const handleNewPassword = async () => {
      if (!newPassword.value) {
        toast.add({
          severity: "warn",
          summary: "Missing Password",
          detail: "Please enter a new password.",
          life: 3000
        });
        return;
      }
      try {
        // Send new password to backend
        await authStore.resetPassword({ 
          Password: newPassword.value 
        });
        toast.add({
          severity: "success",
          summary: "Password Reset Successful",
          detail: "Your password has been reset. You can now log in with your new password.",
          life: 3000
        });
        // Redirect to login page after successful reset
        setTimeout(() => {
          window.location.href = "/login";
        }, 2000);
      } catch (error) {
        toast.add({
          severity: "error",
          summary: "Error",
          detail: "Failed to reset the password. Please try again.",
          life: 3000
        });
      }
    };

    return { 
      forgotUsername, 
      confirmationCode, 
      newPassword, 
      activeStep,
      toast, 
      handleForgotPassword, 
      handleConfirmationCode, 
      handleNewPassword,
      sentCode,
      isCodeVerified
    };
  },
};
</script>

<style scoped>
.forgot-password-page {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: var(--surface-ground);
  padding: 2rem;
}

.forgot-password-card {
  width: 100%;
  max-width: 650px;
  border-radius: 10px;
  padding: 10px;
  box-shadow: var(--card-shadow);
}

.header-content {
  text-align: center;
  padding: 1rem 0;
}

.forgot-password-header-title {
  font-size: 1.8rem;
  margin-bottom: 0.5rem;
  color: var(--text-color);
}

.forgot-password-header-description {
  color: var(--text-color-secondary);
  font-size: 1rem;
}

.step-panel-content {
  display: flex;
  flex-direction: column;
  padding: 1.5rem 1rem;
  min-height: 300px;
}

.form-container {
  flex: 1;
  max-width: 450px;
  margin: 0 auto;
  width: 100%;
  padding: 1.5rem;
  border-radius: var(--border-radius);
  background: var(--surface-card);
  margin-bottom: 2rem;
}

.form-text {
  display: block;
  margin-top: 0.25rem;
  color: var(--text-color-secondary);
}

.navigation-container {
  display: flex;
  justify-content: space-between;
  width: 100%;
  max-width: 450px;
  margin: 0 auto;
}

.navigation-container .p-button {
  min-width: 120px;
}

.mt-3 {
  margin-top: 1rem;
}

.w-full {
  width: 100%;
}

/* Responsive adjustments */
@media (max-width: 768px) {
  .forgot-password-page {
    padding: 1rem;
  }
  
  .forgot-password-card {
    max-width: 100%;
  }
  
  .form-container {
    padding: 1rem;
  }
  
  .navigation-container .p-button {
    min-width: 100px;
  }
}
</style>