export class Destination {
  constructor(data = {}) {
    this.id = data.id || 0;
    this.tripId = data.tripId || 0;
    this.name = data.name || '';
    this.location = data.location || '';
    this.arrivalDate = data.arrivalDate || '';
    this.departureDate = data.departureDate || '';
    this.description = data.description || '';
    this.notes = data.notes || '';
  }
}

export const EMPTY_DESTINATION_FORM = {
  name: '',
  location: '',
  arrivalDate: '',
  departureDate: '',
  description: '',
  notes: '',
};
