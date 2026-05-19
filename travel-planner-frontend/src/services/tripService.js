import { http } from './httpClient';

const tripService = {
  async getAll() {
    return http.get('/trips');
  },

  async getById(id) {
    return http.get(`/trips/${id}`);
  },

  async create(data) {
    return http.post('/trips', data);
  },

  async update(id, data) {
    return http.put(`/trips/${id}`, data);
  },

  async delete(id) {
    return http.delete(`/trips/${id}`);
  },
};

export default tripService;
