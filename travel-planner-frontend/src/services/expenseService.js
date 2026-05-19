import { http } from './httpClient';
import { EXPENSE_CATEGORY_MAP } from '../models/Expense';

// Konvertuje category string u numericki enum koji backend ocekuje
function toDto(data) {
  if (!data) return data;
  const dto = { ...data };
  if (typeof dto.category === 'string') {
    dto.category = EXPENSE_CATEGORY_MAP[dto.category] ?? 5; 
  }
  return dto;
}

const expenseService = {
  async getAllByTrip(tripId) {
    return http.get(`/trips/${tripId}/expenses`);
  },

  async getById(tripId, id) {
    return http.get(`/trips/${tripId}/expenses/${id}`);
  },

  async create(tripId, data) {
    return http.post(`/trips/${tripId}/expenses`, toDto(data));
  },

  async update(tripId, id, data) {
    return http.put(`/trips/${tripId}/expenses/${id}`, toDto(data));
  },

  async delete(tripId, id) {
    return http.delete(`/trips/${tripId}/expenses/${id}`);
  },

  async getTotal(tripId) {
    return http.get(`/trips/${tripId}/expenses/total`);
  },

  // GET /api/trips/{tripId}/expenses/summary
  async getBudgetSummary(tripId) {
    return http.get(`/trips/${tripId}/expenses/summary`);
  },
};

export default expenseService;
