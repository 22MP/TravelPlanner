export class Trip {
  constructor(data = {}) {
    this.id = data.id || 0;
    this.name = data.name || '';
    this.description = data.description || '';
    this.startDate = data.startDate || '';
    this.endDate = data.endDate || '';
    this.budget = data.budget || 0;
    this.notes = data.notes || '';
    this.userId = data.userId || 0;
    this.createdAt = data.createdAt || '';
    
    this.totalExpenses = data.totalExpenses || 0;
    this.remainingBudget = data.remainingBudget ?? data.budget ?? 0;
  }
}

export const EMPTY_TRIP_FORM = {
  title: '',
  description: '',
  startDate: '',
  endDate: '',
  budget: '',
  notes: '',
};
