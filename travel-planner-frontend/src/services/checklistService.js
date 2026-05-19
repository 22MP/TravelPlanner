import { http } from './httpClient';

const checklistService = {
  async getAllByTrip(tripId) {
    return http.get(`/trips/${tripId}/checklist`);
  },

  async create(tripId, data) {
    return http.post(`/trips/${tripId}/checklist`, data);
  },

  async update(tripId, id, data) {
    return http.put(`/trips/${tripId}/checklist/${id}`, data);
  },

  async delete(tripId, id) {
    return http.delete(`/trips/${tripId}/checklist/${id}`);
  },
};

export default checklistService;
