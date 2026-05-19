// ActivityStatus enum na backendu:
// 0 = Planned, 1 = Reserved, 2 = Completed, 3 = Cancelled
export const ACTIVITY_STATUS_MAP = {
  planned: 0,
  reserved: 1,
  completed: 2,
  cancelled: 3,
};

export const ACTIVITY_STATUS_REVERSE_MAP = {
  0: 'planned',
  1: 'reserved',
  2: 'completed',
  3: 'cancelled',
};

export class Activity {
  constructor(data = {}) {
    this.id = data.id || 0;
    this.tripId = data.tripId || 0;
    this.name = data.name || '';
    this.date = data.date || '';
    this.time = data.time || '';
    this.location = data.location || '';
    this.description = data.description || '';
    this.estimatedCost = data.estimatedCost || 0;
    // Backend vraca numericki enum — konvertujemo u string za prikaz
    this.status =
      typeof data.status === 'number'
        ? ACTIVITY_STATUS_REVERSE_MAP[data.status] ?? 'planned'
        : data.status || 'planned';
  }
}

export const ACTIVITY_STATUSES = ['planned', 'reserved', 'completed', 'cancelled'];

export const EMPTY_ACTIVITY_FORM = {
  name: '',
  date: '',
  time: '',
  location: '',
  description: '',
  estimatedCost: '',
  status: 'planned',
};
