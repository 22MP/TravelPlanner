import { http } from './httpClient';

const destinationService = {
  async getAllByTrip(tripId) {
    return http.get(`/trips/${tripId}/destinations`);
  },

  async getById(tripId, id) {
    return http.get(`/trips/${tripId}/destinations/${id}`);
  },

  async create(tripId, data) {
    return http.post(`/trips/${tripId}/destinations`, data);
  },

  async update(tripId, id, data) {
    return http.put(`/trips/${tripId}/destinations/${id}`, data);
  },

  async delete(tripId, id) {
    return http.delete(`/trips/${tripId}/destinations/${id}`);
  },
};

export default destinationService;
