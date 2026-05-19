import { http } from './httpClient';

const authService = {
  async register(data) {
    return http.post('/auth/register', data);
  },

  async login(data) {
    return http.post('/auth/login', data);
  },

  async getAllUsers() {
    return http.get('/auth/users');
  },

  async deleteUser(id) {
    return http.delete(`/auth/users/${id}`);
  },
};

export default authService;
