import { http } from './httpClient';

// AccessType enum na backendu: 0 = VIEW, 1 = EDIT
const ACCESS_TYPE_MAP = {
  view: 0,
  edit: 1,
};

const sharedLinkService = {
  async getAllByTrip(tripId) {
    return http.get(`/trips/${tripId}/shared-links`);
  },

  async create(tripId, data) {
    // data.accessType je string 'view'/'edit', backend ocekuje numericki enum
    const dto = {
      ...data,
      accessType: ACCESS_TYPE_MAP[data.accessType?.toLowerCase()] ?? 0,
    };
    return http.post(`/trips/${tripId}/shared-links`, dto);
  },

  async delete(tripId, id) {
    return http.delete(`/trips/${tripId}/shared-links/${id}`);
  },

  // GET /api/shared/{token} — definisano van trips rute u SharedLinksController
  async getByToken(token) {
    return http.get(`/shared/${token}`);
  },

  // GET /api/shared/{token}/validate?accessType=VIEW
  async validateToken(token, accessType) {
    // Backend ocekuje string "VIEW" ili "EDIT" u query parametru
    const accessTypeStr = accessType?.toUpperCase() || 'VIEW';
    return http.get(`/shared/${token}/validate?accessType=${accessTypeStr}`);
  },
};

export default sharedLinkService;
