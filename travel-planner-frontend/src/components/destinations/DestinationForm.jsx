import { useState } from 'react';
import { EMPTY_DESTINATION_FORM } from '../../models/Destination';

function validate(form) {
  const errors = {};
  if (!form.name.trim()) errors.name = 'Name is required.';
  if (form.arrivalDate && form.departureDate && form.departureDate < form.arrivalDate) {
    errors.departureDate = 'Departure cannot be before arrival.';
  }
  return errors;
}

export default function DestinationForm({ initialData, onSubmit, onCancel, loading }) {
  const [form, setForm] = useState(
    initialData
      ? {
          name: initialData.name || '',
          location: initialData.location || '',
          arrivalDate: initialData.arrivalDate?.split('T')[0] || '',
          departureDate: initialData.departureDate?.split('T')[0] || '',
          description: initialData.description || '',
          notes: initialData.notes || '',
        }
      : EMPTY_DESTINATION_FORM
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
    onSubmit(form);
  }

  return (
    <form onSubmit={handleSubmit} noValidate>
      <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
        <div className="form-group">
          <label className="form-label">Destination name *</label>
          <input
            className={`form-input ${errors.name ? 'error' : ''}`}
            name="name"
            value={form.name}
            onChange={handleChange}
            placeholder="e.g. Rome"
          />
          {errors.name && <span className="form-error">{errors.name}</span>}
        </div>

        <div className="form-group">
          <label className="form-label">Location</label>
          <input
            className="form-input"
            name="location"
            value={form.location}
            onChange={handleChange}
            placeholder="e.g. Rome, Italy"
          />
        </div>

        <div className="grid-2">
          <div className="form-group">
            <label className="form-label">Arrival date</label>
            <input
              type="date"
              className="form-input"
              name="arrivalDate"
              value={form.arrivalDate}
              onChange={handleChange}
            />
          </div>
          <div className="form-group">
            <label className="form-label">Departure date</label>
            <input
              type="date"
              className={`form-input ${errors.departureDate ? 'error' : ''}`}
              name="departureDate"
              value={form.departureDate}
              min={form.arrivalDate}
              onChange={handleChange}
            />
            {errors.departureDate && <span className="form-error">{errors.departureDate}</span>}
          </div>
        </div>

        <div className="form-group">
          <label className="form-label">Description</label>
          <textarea
            className="form-textarea"
            name="description"
            value={form.description}
            onChange={handleChange}
            placeholder="What will you do here?"
          />
        </div>

        <div className="form-group">
          <label className="form-label">Notes</label>
          <input
            className="form-input"
            name="notes"
            value={form.notes}
            onChange={handleChange}
            placeholder="Any additional notes..."
          />
        </div>

        <div className="modal-actions">
          <button type="button" className="btn btn-secondary" onClick={onCancel}>
            Cancel
          </button>
          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? 'Saving...' : initialData ? 'Save changes' : 'Add destination'}
          </button>
        </div>
      </div>
    </form>
  );
}
