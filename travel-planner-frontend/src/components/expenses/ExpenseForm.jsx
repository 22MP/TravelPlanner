import { useState } from 'react';
import { EMPTY_EXPENSE_FORM, EXPENSE_CATEGORIES } from '../../models/Expense';

function validate(form) {
  const errors = {};
  if (!form.name.trim()) errors.name = 'Name is required.';
  if (!form.amount || Number(form.amount) <= 0) errors.amount = 'Amount must be greater than 0.';
  if (!form.date) errors.date = 'Date is required.';
  return errors;
}

export default function ExpenseForm({ initialData, onSubmit, onCancel, loading }) {
  const [form, setForm] = useState(
    initialData
      ? {
          name: initialData.name || '',
          category: initialData.category || 'transport',
          amount: initialData.amount ?? '',
          date: initialData.date?.split('T')[0] || '',
          description: initialData.description || '',
        }
      : EMPTY_EXPENSE_FORM
  );
  const [errors, setErrors] = useState({});

  function handleChange(e) {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
    if (errors[name]) setErrors((prev) => ({ ...prev, [name]: '' }));
  }

  function handleSubmit(e) {
    e.preventDefault();
    const validationErrors = validate(form);
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }
    onSubmit({ ...form, amount: Number(form.amount) });
  }

  return (
    <form onSubmit={handleSubmit} noValidate>
      <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
        <div className="form-group">
          <label className="form-label">Name *</label>
          <input
            className={`form-input ${errors.name ? 'error' : ''}`}
            name="name"
            value={form.name}
            onChange={handleChange}
            placeholder="e.g. Train tickets"
          />
          {errors.name && <span className="form-error">{errors.name}</span>}
        </div>

        <div className="grid-2">
          <div className="form-group">
            <label className="form-label">Category</label>
            <select
              className="form-select"
              name="category"
              value={form.category}
              onChange={handleChange}
            >
              {EXPENSE_CATEGORIES.map((c) => (
                <option key={c} value={c}>
                  {c.charAt(0).toUpperCase() + c.slice(1)}
                </option>
              ))}
            </select>
          </div>
          <div className="form-group">
            <label className="form-label">Amount ($) *</label>
            <input
              type="number"
              className={`form-input ${errors.amount ? 'error' : ''}`}
              name="amount"
              value={form.amount}
              onChange={handleChange}
              placeholder="0"
              min="0.01"
              step="0.01"
            />
            {errors.amount && <span className="form-error">{errors.amount}</span>}
          </div>
        </div>

        <div className="form-group">
          <label className="form-label">Date *</label>
          <input
            type="date"
            className={`form-input ${errors.date ? 'error' : ''}`}
            name="date"
            value={form.date}
            onChange={handleChange}
          />
          {errors.date && <span className="form-error">{errors.date}</span>}
        </div>

        <div className="form-group">
          <label className="form-label">Description</label>
          <input
            className="form-input"
            name="description"
            value={form.description}
            onChange={handleChange}
            placeholder="Optional note..."
          />
        </div>

        <div className="modal-actions">
          <button type="button" className="btn btn-secondary" onClick={onCancel}>
            Cancel
          </button>
          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? 'Saving...' : initialData ? 'Save changes' : 'Add expense'}
          </button>
        </div>
      </div>
    </form>
  );
}
