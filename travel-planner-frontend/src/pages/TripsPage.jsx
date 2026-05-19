import { useState, useEffect } from 'react';
import tripService from '../services/tripService';
import { useTripContext } from '../context/TripContext';
import TripCard from '../components/trips/TripCard';
import TripForm from '../components/trips/TripForm';
import './TripsPage.css';
import { Trip } from '../models/Trip';

export default function TripsPage() {
  const { trips, dispatch } = useTripContext();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [formLoading, setFormLoading] = useState(false);
  const [formError, setFormError] = useState('');

  useEffect(() => {
    tripService
      .getAll()
      .then((data) => dispatch({ type: 'SET_TRIPS', payload: data.map(t => new Trip(t)) }))
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  async function handleCreate(formData) {
    setFormLoading(true);
    setFormError('');
    try {
      const trip = await tripService.create(formData);
      dispatch({ type: 'ADD_TRIP', payload: trip });
      setShowModal(false);
    } catch (err) {
      setFormError(err.message);
    } finally {
      setFormLoading(false);
    }
  }

  async function handleDelete(id) {
    try {
      await tripService.delete(id);
      dispatch({ type: 'DELETE_TRIP', payload: id });
    } catch (err) {
      setError(err.message);
    }
  }

  return (
    <div className="trips-page">
      <div className="trips-page-header">
        <div className="page-header">
          <h1>My Trips</h1>
          <p>Plan and organize all your travels in one place.</p>
        </div>
        <button className="btn btn-primary" onClick={() => setShowModal(true)}>
          New trip
        </button>
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      {loading ? (
        <div className="loading-center"><div className="spinner" /></div>
      ) : trips.length === 0 ? (
        <div className="empty-state">
          <h3>No trips yet</h3>
          <p>Start planning your first adventure.</p>
          <button className="btn btn-primary" onClick={() => setShowModal(true)}>
            Create your first trip
          </button>
        </div>
      ) : (
        <div className="trips-grid">
          {trips.map((trip) => (
            <TripCard key={trip.id} trip={trip} onDelete={handleDelete} />
          ))}
        </div>
      )}

      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>New trip</h2>
              <button className="modal-close" onClick={() => setShowModal(false)}>×</button>
            </div>
            {formError && <div className="alert alert-error">{formError}</div>}
            <TripForm
              onSubmit={handleCreate}
              onCancel={() => setShowModal(false)}
              loading={formLoading}
            />
          </div>
        </div>
      )}
    </div>
  );
}
