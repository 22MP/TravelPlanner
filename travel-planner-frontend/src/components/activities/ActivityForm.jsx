import { useState } from 'react';
import { EMPTY_ACTIVITY_FORM, ACTIVITY_STATUSES } from '../../models/Activity';

function validate(form) {
  const errors = {};
  if (!form.name.trim()) errors.name = 'Name is required.';
  if (!form.date) errors.date = 'Date is required.';
  if (form.estimatedCost !== '' && Number(form.estimatedCost) < 0) {
    errors.estimatedCost = 'Cost cannot be negative.';
  }
  return errors;
}

export default function ActivityForm({ initialData, onSubmit, onCancel, loading }) {
  const [form, setForm] = useState(
    initialData
      ? {
          name: initialData.name || '',
          date: initialData.date?.split('T')[0] || '',
          time: initialData.time || '',
          location: initialData.location || '',
          description: initialData.description || '',
          estimatedCost: initialData.estimatedCost ?? '',
          status: initialData.status || 'planned',
        }
      : EMPTY_ACTIVITY_FORM
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
      estimatedCost: form.estimatedCost !== '' ? Number(form.estimatedCost) : 0,
    });
  }

  return (
    <form onSubmit={handleSubmit} noValidate>
      <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
        <div className="form-group">
          <label className="form-label">Activity name *</label>
          <input
            className={`form-input ${errors.name ? 'error' : ''}`}
            name="name"
            value={form.name}
            onChange={handleChange}
            placeholder="e.g. Visit Colosseum"
          />
          {errors.name && <span className="form-error">{errors.name}</span>}
        </div>

        <div className="grid-2">
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
            <label className="form-label">Time</label>
            <input
              type="time"
              className="form-input"
              name="time"
              value={form.time}
              onChange={handleChange}
            />
          </div>
        </div>

        <div className="form-group">
          <label className="form-label">Location</label>
          <input
            className="form-input"
            name="location"
            value={form.location}
            onChange={handleChange}
            placeholder="e.g. Piazza del Colosseo, Rome"
          />
        </div>

        <div className="grid-2">
          <div className="form-group">
            <label className="form-label">Estimated cost ($)</label>
            <input
              type="number"
              className={`form-input ${errors.estimatedCost ? 'error' : ''}`}
              name="estimatedCost"
              value={form.estimatedCost}
              onChange={handleChange}
              placeholder="0"
              min="0"
            />
            {errors.estimatedCost && <span className="form-error">{errors.estimatedCost}</span>}
          </div>
          <div className="form-group">
            <label className="form-label">Status</label>
            <select
              className="form-select"
              name="status"
              value={form.status}
              onChange={handleChange}
            >
              {ACTIVITY_STATUSES.map((s) => (
                <option key={s} value={s}>
                  {s.charAt(0).toUpperCase() + s.slice(1)}
                </option>
              ))}
            </select>
          </div>
        </div>

        <div className="form-group">
          <label className="form-label">Description</label>
          <textarea
            className="form-textarea"
            name="description"
            value={form.description}
            onChange={handleChange}
            placeholder="Additional details..."
          />
        </div>

        <div className="modal-actions">
          <button type="button" className="btn btn-secondary" onClick={onCancel}>
            Cancel
          </button>
          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? 'Saving...' : initialData ? 'Save changes' : 'Add activity'}
          </button>
        </div>
      </div>
    </form>
  );
}
