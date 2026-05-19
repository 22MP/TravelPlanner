import { useNavigate } from 'react-router-dom';
import './TripCard.css';

function formatDate(dateStr) {
  if (!dateStr) return '';
  return new Date(dateStr).toLocaleDateString('en-GB', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
  });
}

function getTripDuration(startDate, endDate) {
  if (!startDate || !endDate) return null;
  const diff = new Date(endDate) - new Date(startDate);
  const days = Math.ceil(diff / (1000 * 60 * 60 * 24));
  return days > 0 ? `${days} day${days !== 1 ? 's' : ''}` : null;
}

export default function TripCard({ trip, onDelete }) {
  const navigate = useNavigate();
  const duration = getTripDuration(trip.startDate, trip.endDate);

  function handleDelete(e) {
    e.stopPropagation();
    if (window.confirm(`Delete "${trip.name}"? This cannot be undone.`)) {
      onDelete(trip.id);
    }
  }

  return (
    <article className="trip-card" onClick={() => navigate(`/trips/${trip.id}`)}>
      <div className="trip-card-body">
        <h3 className="trip-card-title">{trip.name}</h3>
        {trip.description && (
          <p className="trip-card-desc">{trip.description}</p>
        )}
        <div className="trip-card-meta">
          {trip.startDate && (
            <span className="trip-meta-item">
              {formatDate(trip.startDate)}
              {trip.endDate && ` — ${formatDate(trip.endDate)}`}
            </span>
          )}
          {duration && <span className="trip-meta-badge">{duration}</span>}
        </div>
        {trip.budget > 0 && (
          <div className="trip-card-budget">
            Budget: <strong>${Number(trip.budget).toLocaleString()}</strong>
          </div>
        )}
      </div>
      <div className="trip-card-actions">
        <button
          className="btn btn-ghost btn-sm"
          onClick={(e) => {
            e.stopPropagation();
            navigate(`/trips/${trip.id}`);
          }}
        >
          View
        </button>
        <button
          className="btn btn-sm"
          style={{ color: 'var(--color-danger)', background: 'var(--color-danger-light)' }}
          onClick={handleDelete}
        >
          Delete
        </button>
      </div>
    </article>
  );
}