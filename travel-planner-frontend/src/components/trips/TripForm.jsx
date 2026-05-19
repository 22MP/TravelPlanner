import { useState } from 'react';
import { EMPTY_TRIP_FORM } from '../../models/Trip';

function validate(form) {
  const errors = {};
  if (!form.name.trim()) errors.name = 'Name is required.';
  if (!form.startDate) errors.startDate = 'Start date is required.';
  if (!form.endDate) errors.endDate = 'End date is required.';
  if (form.startDate && form.endDate && form.endDate < form.startDate) {
    errors.endDate = 'End date cannot be before start date.';
  }
  if (form.budget !== '' && Number(form.budget) < 0) {
    errors.budget = 'Budget cannot be negative.';
  }
  return errors;
}

export default function TripForm({ initialData, onSubmit, onCancel, loading }) {
  const [form, setForm] = useState(
    initialData
      ? {
          name: initialData.name || '',
          description: initialData.description || '',
          startDate: initialData.startDate?.split('T')[0] || '',
          endDate: initialData.endDate?.split('T')[0] || '',
          budget: initialData.budget ?? '',
          notes: initialData.notes || '',
        }
      : EMPTY_TRIP_FORM
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
    onSubmit({
      ...form,
      budget: form.budget !== '' ? Number(form.budget) : 0,
    });
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
            placeholder="e.g. Summer in Italy"
          />
          {errors.name && <span className="form-error">{errors.name}</span>}
        </div>

        <div className="form-group">
          <label className="form-label">Description</label>
          <textarea
            className="form-textarea"
            name="description"
            value={form.description}
            onChange={handleChange}
            placeholder="Brief overview of the trip..."
          />
        </div>

        <div className="grid-2">
          <div className="form-group">
            <label className="form-label">Start date *</label>
            <input
              type="date"
              className={`form-input ${errors.startDate ? 'error' : ''}`}
              name="startDate"
              value={form.startDate}
              onChange={handleChange}
            />
            {errors.startDate && <span className="form-error">{errors.startDate}</span>}
          </div>
          <div className="form-group">
            <label className="form-label">End date *</label>
            <input
              type="date"
              className={`form-input ${errors.endDate ? 'error' : ''}`}
              name="endDate"
              value={form.endDate}
              min={form.startDate}
              onChange={handleChange}
            />
            {errors.endDate && <span className="form-error">{errors.endDate}</span>}
          </div>
        </div>

        <div className="form-group">
          <label className="form-label">Budget ($)</label>
          <input
            type="number"
            className={`form-input ${errors.budget ? 'error' : ''}`}
            name="budget"
            value={form.budget}
            onChange={handleChange}
            placeholder="0"
            min="0"
          />
          {errors.budget && <span className="form-error">{errors.budget}</span>}
        </div>

        <div className="form-group">
          <label className="form-label">Notes</label>
          <textarea
            className="form-textarea"
            name="notes"
            value={form.notes}
            onChange={handleChange}
            placeholder="Any general notes or reminders..."
          />
        </div>

        <div className="modal-actions">
          <button type="button" className="btn btn-secondary" onClick={onCancel}>
            Cancel
          </button>
          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? 'Saving...' : initialData ? 'Save changes' : 'Create trip'}
          </button>
        </div>
      </div>
    </form>
  );
}