import axios from "axios";

class ApiService {
  static api = axios.create({
    baseURL: "http://localhost:8082",
    headers: {
      "Content-Type": "application/json",
    },
  });

  static setAuthToken(token) {
    if (token) {
      this.api.defaults.headers.Authorization = `Bearer ${token}`;
    } else {
      delete this.api.defaults.headers.Authorization;
    }
  }

  // Auth Endpoints
  static async register(user) {
    try {
      const response = await this.api.post("/api/auth/register", user);
      return response;
    } catch (error) {
      return error;
    }
  }

  static async login(credentials) {
    try {
      const response = await this.api.post("/api/auth/login", credentials);
      return await response.data;
    } catch (error) {
      throw await error.response?.data || error.message;
    }
  }

  static async forgotPassword(credentials) {
    try {
      const response = await this.api.post("/api/auth/forgot-password", credentials);
      return response.data.token;
    } catch (error) {
      throw await error.response?.data || error.message;
    }
  }

  static async confirmEmail(token, code){
    try {
      const response = await this.api.get("/api/auth/confirm-email", { 
        params: { 
          token: token, 
          confirmationCode: code
        }
      });
      return response;
    } catch (error) {
      throw await error.response?.data || error.message;
    }
  }

  static async verifyResetCode(token, code){
    try {
      const response = await this.api.post("/api/auth/verify-reset-code", code, { 
        params: { 
          token: token,
        }
      });
      return response;
    } catch (error) {
      throw await error.response?.data || error.message;
    }
  }

  static async resetPassowrd(token, password){
    try {
      const response = await this.api.post("/api/auth/reset-password", password, { 
        params: { 
          token: token, 
        }
      });
      return response;
    } catch (error) {
      throw await error.response?.data || error.message;
    }
  }


  static async changePassword(changePasswordRequest) {
    try {
      const response = await this.api.post("/api/auth/change-password", changePasswordRequest);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  }

  // User Endpoints
  static async getUserProfile(userId) {
    try {
      const response = await this.api.get(`/api/users/${userId}/data`);
      console.log(response.data);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  }

  // Chat Endpoints
  static async getUserChats(userId) {
    try {
      const response = await this.api.get(`/api/users/${userId}/chats`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  }

  static async getUserChat(userId, chatId) {
    try {
      const response = await this.api.get(`/api/users/${userId}/chats/${chatId}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  }

  static async createChat(userId, chatCreateDto) {
    try {
      const response = await this.api.post(`/api/users/${userId}/chats`, chatCreateDto);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  }

  static async updateChat(userId, chatUpdateDto) {
    try {
      const response = await this.api.put(`/api/users/${userId}/chats`, chatUpdateDto);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  }

  static async deleteChat(userId, chatId) {
    try {
      const response = await this.api.delete(`/api/users/${userId}/chats/${chatId}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  }

  // Message Endpoints
  static async postMessage(userId, chatId, messageCreateDto) {
    try {
      const response = await this.api.post(`/api/users/${userId}/chats/${chatId}`, messageCreateDto);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  }
}

export default ApiService;
