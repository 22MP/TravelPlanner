// ExpenseCategory enum na backendu:
// 0 = Transport, 1 = Accommodation, 2 = Food, 3 = Tickets, 4 = Shopping, 5 = Other
export const EXPENSE_CATEGORY_MAP = {
  transport: 0,
  accommodation: 1,
  food: 2,
  tickets: 3,
  shopping: 4,
  other: 5,
};

export const EXPENSE_CATEGORY_REVERSE_MAP = {
  0: 'transport',
  1: 'accommodation',
  2: 'food',
  3: 'tickets',
  4: 'shopping',
  5: 'other',
};

export class Expense {
  constructor(data = {}) {
    this.id = data.id || 0;
    this.tripId = data.tripId || 0;
    this.name = data.name || '';
    // Backend vraca numericki enum — konvertujemo u string za prikaz
    this.category =
      typeof data.category === 'number'
        ? EXPENSE_CATEGORY_REVERSE_MAP[data.category] ?? 'other'
        : data.category || 'other';
    this.amount = data.amount || 0;
    this.date = data.date || '';
    this.description = data.description || '';
  }
}

export const EXPENSE_CATEGORIES = [
  'transport',
  'accommodation',
  'food',
  'tickets',
  'shopping',
  'other',
];

export const EMPTY_EXPENSE_FORM = {
  name: '',
  category: 'transport',
  amount: '',
  date: '',
  description: '',
};
