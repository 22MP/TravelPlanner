import { http } from './httpClient';
import { ACTIVITY_STATUS_MAP } from '../models/Activity';


function toDto(data) {
  if (!data) return data;
  const dto = { ...data };

  
  if (typeof dto.status === 'string') {
    dto.status = ACTIVITY_STATUS_MAP[dto.status] ?? 0;
  }

  
  if (!dto.time || dto.time.trim() === '') {
    dto.time = '00:00:00';
  } else if (/^\d{2}:\d{2}$/.test(dto.time)) {
    dto.time = dto.time + ':00';
  }

  return dto;
}

const activityService = {
  async getAllByTrip(tripId) {
    return http.get(`/trips/${tripId}/activities`);
  },

  async getById(tripId, id) {
    return http.get(`/trips/${tripId}/activities/${id}`);
  },

  async create(tripId, data) {
    return http.post(`/trips/${tripId}/activities`, toDto(data));
  },

  async update(tripId, id, data) {
    return http.put(`/trips/${tripId}/activities/${id}`, toDto(data));
  },

  async delete(tripId, id) {
    return http.delete(`/trips/${tripId}/activities/${id}`);
  },
};

export default activityService;