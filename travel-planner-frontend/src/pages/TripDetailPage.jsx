import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useTripContext } from '../context/TripContext';
import tripService from '../services/tripService';
import destinationService from '../services/destinationService';
import activityService from '../services/activityService';
import expenseService from '../services/expenseService';
import checklistService from '../services/checklistService';
import { Expense } from '../models/Expense';
import { Activity } from '../models/Activity';
import TripForm from '../components/trips/TripForm';
import DestinationForm from '../components/destinations/DestinationForm';
import ActivityForm from '../components/activities/ActivityForm';
import ExpenseForm from '../components/expenses/ExpenseForm';
import ChecklistItem from '../components/checklist/ChecklistItem';
import SharedLinks from '../components/shared/SharedLinks';
import '../css/TripDetailPage.css';

const TABS = ['overview', 'destinations', 'activities', 'expenses', 'checklist', 'sharing'];

function formatDate(dateStr) {
  if (!dateStr) return '';
  return new Date(dateStr).toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' });
}

function groupByDate(items) {
  return items.reduce((acc, item) => {
    const date = item.date?.split('T')[0] || 'No date';
    if (!acc[date]) acc[date] = [];
    acc[date].push(item);
    return acc;
  }, {});
}

export default function TripDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { currentTrip, destinations, activities, expenses, checklist, dispatch } = useTripContext();

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [activeTab, setActiveTab] = useState('overview');

  const [editTripModal, setEditTripModal] = useState(false);
  const [destModal, setDestModal] = useState(null);
  const [actModal, setActModal] = useState(null);
  const [expModal, setExpModal] = useState(null);
  const [newChecklistItem, setNewChecklistItem] = useState('');

  const [formLoading, setFormLoading] = useState(false);
  const [formError, setFormError] = useState('');

  const tripId = Number(id);

  useEffect(() => {
    async function loadAll() {
      setLoading(true);
      setError('');
      try {
        const [trip, dests, acts, exps, checks] = await Promise.all([
          tripService.getById(tripId),
          destinationService.getAllByTrip(tripId),
          activityService.getAllByTrip(tripId),
          expenseService.getAllByTrip(tripId),
          checklistService.getAllByTrip(tripId),
        ]);
        dispatch({ type: 'SET_CURRENT_TRIP', payload: trip });
        dispatch({ type: 'SET_DESTINATIONS', payload: dests });
        dispatch({ type: 'SET_ACTIVITIES', payload: acts.map((a) => new Activity(a)) });
        dispatch({ type: 'SET_EXPENSES', payload: exps.map((e) => new Expense(e)) });
        dispatch({ type: 'SET_CHECKLIST', payload: checks });
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }
    loadAll();
  }, [tripId]);

  async function handleUpdateTrip(formData) {
    setFormLoading(true); setFormError('');
    try {
      const updated = await tripService.update(tripId, formData);
      dispatch({ type: 'UPDATE_TRIP', payload: updated });
      setEditTripModal(false);
    } catch (err) { setFormError(err.message); }
    finally { setFormLoading(false); }
  }

  async function handleSaveDestination(formData) {
    setFormLoading(true); setFormError('');
    try {
      if (destModal?.id) {
        const updated = await destinationService.update(tripId, destModal.id, formData);
        dispatch({ type: 'UPDATE_DESTINATION', payload: updated });
      } else {
        const created = await destinationService.create(tripId, formData);
        dispatch({ type: 'ADD_DESTINATION', payload: created });
      }
      setDestModal(null);
    } catch (err) { setFormError(err.message); }
    finally { setFormLoading(false); }
  }

  async function handleDeleteDestination(destId) {
    if (!window.confirm('Delete this destination?')) return;
    try {
      await destinationService.delete(tripId, destId);
      dispatch({ type: 'DELETE_DESTINATION', payload: destId });
    } catch (err) { setError(err.message); }
  }

  async function handleSaveActivity(formData) {
    setFormLoading(true); setFormError('');
    try {
      if (actModal?.id) {
        const updated = await activityService.update(tripId, actModal.id, formData);
        dispatch({ type: 'UPDATE_ACTIVITY', payload: new Activity(updated) });
      } else {
        const created = await activityService.create(tripId, formData);
        dispatch({ type: 'ADD_ACTIVITY', payload: new Activity(created) });
      }
      setActModal(null);
    } catch (err) { setFormError(err.message); }
    finally { setFormLoading(false); }
  }

  async function handleDeleteActivity(actId) {
    if (!window.confirm('Delete this activity?')) return;
    try {
      await activityService.delete(tripId, actId);
      dispatch({ type: 'DELETE_ACTIVITY', payload: actId });
    } catch (err) { setError(err.message); }
  }

  async function handleSaveExpense(formData) {
    setFormLoading(true); setFormError('');
    try {
      if (expModal?.id) {
        const updated = await expenseService.update(tripId, expModal.id, formData);
        dispatch({ type: 'UPDATE_EXPENSE', payload: new Expense(updated) });
      } else {
        const created = await expenseService.create(tripId, formData);
        dispatch({ type: 'ADD_EXPENSE', payload: new Expense(created) });
      }
      setExpModal(null);
    } catch (err) { setFormError(err.message); }
    finally { setFormLoading(false); }
  }

  async function handleDeleteExpense(expId) {
    if (!window.confirm('Delete this expense?')) return;
    try {
      await expenseService.delete(tripId, expId);
      dispatch({ type: 'DELETE_EXPENSE', payload: expId });
    } catch (err) { setError(err.message); }
  }

  async function handleAddChecklistItem(e) {
    e.preventDefault();
    if (!newChecklistItem.trim()) return;
    try {
      const item = await checklistService.create(tripId, { name: newChecklistItem.trim() });
      dispatch({ type: 'ADD_CHECKLIST_ITEM', payload: item });
      setNewChecklistItem('');
    } catch (err) { setError(err.message); }
  }

  async function handleToggleChecklistItem(item) {
    try {
      const updated = await checklistService.update(tripId, item.id, { isCompleted: !item.isCompleted });
      dispatch({ type: 'UPDATE_CHECKLIST_ITEM', payload: updated });
    } catch (err) { setError(err.message); }
  }

  async function handleDeleteChecklistItem(itemId) {
    try {
      await checklistService.delete(tripId, itemId);
      dispatch({ type: 'DELETE_CHECKLIST_ITEM', payload: itemId });
    } catch (err) { setError(err.message); }
  }

  function closeModal() {
    setDestModal(null); setActModal(null); setExpModal(null);
    setEditTripModal(false); setFormError('');
  }

  const totalExpenses = expenses.reduce((sum, e) => sum + Number(e.amount || 0), 0);
  const remaining = currentTrip ? Number(currentTrip.budget) - totalExpenses : 0;
  const activitiesByDate = groupByDate(activities);

  if (loading) return <div className="loading-center"><div className="spinner" /></div>;
  if (!currentTrip) return <div className="trips-page"><div className="alert alert-error">{error || 'Trip not found.'}</div></div>;

  const tripTitle = currentTrip.name || '';

  return (
    <div className="detail-page">
      <div className="detail-header">
        <button className="btn btn-ghost btn-sm" onClick={() => navigate('/trips')}>
          Back to trips
        </button>
        <div className="detail-title-row">
          <h1>{tripTitle}</h1>
          <button className="btn btn-secondary btn-sm" onClick={() => setEditTripModal(true)}>
            Edit trip
          </button>
        </div>
        {currentTrip.description && <p className="detail-desc">{currentTrip.description}</p>}
        <div className="detail-meta">
          {currentTrip.startDate && (
            <span>{formatDate(currentTrip.startDate)} — {formatDate(currentTrip.endDate)}</span>
          )}
          {currentTrip.budget > 0 && (
            <span>Budget: <strong>${Number(currentTrip.budget).toLocaleString()}</strong></span>
          )}
        </div>
      </div>

      {error && <div className="alert alert-error" style={{ maxWidth: 1100, margin: '0 auto 16px', padding: '0 24px' }}>{error}</div>}

      <div className="detail-tabs-wrapper">
        <div className="tabs detail-tabs">
          {TABS.map((tab) => (
            <button
              key={tab}
              className={`tab ${activeTab === tab ? 'active' : ''}`}
              onClick={() => setActiveTab(tab)}
            >
              {tab.charAt(0).toUpperCase() + tab.slice(1)}
            </button>
          ))}
        </div>
      </div>

      <div className="detail-content">
        {activeTab === 'overview' && (
          <div className="overview-grid">
            <div className="card">
              <h3 className="section-title">Trip summary</h3>
              <div className="divider" />
              <div className="overview-stats">
                <div className="stat">
                  <span className="stat-value">{destinations.length}</span>
                  <span className="stat-label">Destinations</span>
                </div>
                <div className="stat">
                  <span className="stat-value">{activities.length}</span>
                  <span className="stat-label">Activities</span>
                </div>
                <div className="stat">
                  <span className="stat-value">{expenses.length}</span>
                  <span className="stat-label">Expenses</span>
                </div>
                <div className="stat">
                  <span className="stat-value">{checklist.filter((c) => c.isCompleted).length}/{checklist.length}</span>
                  <span className="stat-label">Checklist</span>
                </div>
              </div>
            </div>
            {currentTrip.budget > 0 && (
              <div className="card">
                <h3 className="section-title">Budget</h3>
                <div className="divider" />
                <div className="budget-bar-container">
                  <div className="budget-bar">
                    <div
                      className="budget-bar-fill"
                      style={{
                        width: `${Math.min((totalExpenses / currentTrip.budget) * 100, 100)}%`,
                        background: remaining < 0 ? 'var(--color-danger)' : 'var(--color-accent)',
                      }}
                    />
                  </div>
                  <div className="budget-labels">
                    <span>Spent: <strong>${totalExpenses.toLocaleString()}</strong></span>
                    <span style={{ color: remaining < 0 ? 'var(--color-danger)' : 'var(--color-success)' }}>
                      Remaining: <strong>${remaining.toLocaleString()}</strong>
                    </span>
                  </div>
                </div>
              </div>
            )}
            {currentTrip.notes && (
              <div className="card">
                <h3 className="section-title">Notes</h3>
                <div className="divider" />
                <p className="notes-text">{currentTrip.notes}</p>
              </div>
            )}
          </div>
        )}

        {activeTab === 'destinations' && (
          <div className="section-panel">
            <div className="section-panel-header">
              <h2 className="section-title">Destinations</h2>
              <button className="btn btn-primary btn-sm" onClick={() => setDestModal({})}>Add destination</button>
            </div>
            {destinations.length === 0 ? (
              <div className="empty-state"><h3>No destinations added</h3><p>Add the places you plan to visit.</p></div>
            ) : (
              <div className="items-list">
                {destinations.map((dest) => (
                  <div key={dest.id} className="item-card">
                    <div className="item-card-body">
                      <h4>{dest.name}</h4>
                      {dest.location && <p className="item-meta">{dest.location}</p>}
                      {dest.arrivalDate && (
                        <p className="item-meta">{formatDate(dest.arrivalDate)} — {formatDate(dest.departureDate)}</p>
                      )}
                      {dest.description && <p className="item-desc">{dest.description}</p>}
                    </div>
                    <div className="item-card-actions">
                      <button className="btn btn-ghost btn-sm" onClick={() => setDestModal(dest)}>Edit</button>
                      <button className="btn btn-sm" style={{ color: 'var(--color-danger)', background: 'var(--color-danger-light)' }} onClick={() => handleDeleteDestination(dest.id)}>Delete</button>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        )}

        {activeTab === 'activities' && (
          <div className="section-panel">
            <div className="section-panel-header">
              <h2 className="section-title">Activities</h2>
              <button className="btn btn-primary btn-sm" onClick={() => setActModal({})}>Add activity</button>
            </div>
            {activities.length === 0 ? (
              <div className="empty-state"><h3>No activities planned</h3><p>Add things to do on your trip.</p></div>
            ) : (
              <div className="activities-by-date">
                {Object.entries(activitiesByDate).sort(([a], [b]) => a.localeCompare(b)).map(([date, acts]) => (
                  <div key={date} className="date-group">
                    <h4 className="date-group-label">{date === 'No date' ? date : formatDate(date)}</h4>
                    <div className="items-list">
                      {acts.map((act) => (
                        <div key={act.id} className="item-card">
                          <div className="item-card-body">
                            <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                              <h4>{act.name}</h4>
                              <span className={`badge badge-${act.status}`}>{act.status}</span>
                            </div>
                            {act.time && <p className="item-meta">{act.time}</p>}
                            {act.location && <p className="item-meta">{act.location}</p>}
                            {act.estimatedCost > 0 && <p className="item-meta">Est. cost: ${act.estimatedCost}</p>}
                            {act.description && <p className="item-desc">{act.description}</p>}
                          </div>
                          <div className="item-card-actions">
                            <button className="btn btn-ghost btn-sm" onClick={() => setActModal(act)}>Edit</button>
                            <button className="btn btn-sm" style={{ color: 'var(--color-danger)', background: 'var(--color-danger-light)' }} onClick={() => handleDeleteActivity(act.id)}>Delete</button>
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        )}

        {activeTab === 'expenses' && (
          <div className="section-panel">
            <div className="section-panel-header">
              <h2 className="section-title">Expenses</h2>
              <button className="btn btn-primary btn-sm" onClick={() => setExpModal({})}>Add expense</button>
            </div>
            {currentTrip.budget > 0 && (
              <div className="expense-summary card" style={{ marginBottom: '20px' }}>
                <div className="expense-summary-row">
                  <span>Total expenses</span>
                  <strong>${totalExpenses.toLocaleString()}</strong>
                </div>
                <div className="expense-summary-row">
                  <span>Budget</span>
                  <strong>${Number(currentTrip.budget).toLocaleString()}</strong>
                </div>
                <div className="divider" style={{ margin: '12px 0' }} />
                <div className="expense-summary-row">
                  <span>Remaining</span>
                  <strong style={{ color: remaining < 0 ? 'var(--color-danger)' : 'var(--color-success)' }}>
                    ${remaining.toLocaleString()}
                  </strong>
                </div>
              </div>
            )}
            {expenses.length === 0 ? (
              <div className="empty-state"><h3>No expenses logged</h3><p>Track your spending here.</p></div>
            ) : (
              <div className="items-list">
                {expenses.map((exp) => (
                  <div key={exp.id} className="item-card">
                    <div className="item-card-body">
                      <h4>{exp.name}</h4>
                      <div style={{ display: 'flex', gap: '10px', alignItems: 'center' }}>
                        <span className="badge" style={{ background: 'var(--color-surface-alt)', color: 'var(--color-text-secondary)', border: '1px solid var(--color-border)' }}>{exp.category}</span>
                        {exp.date && <span className="item-meta">{formatDate(exp.date)}</span>}
                      </div>
                      {exp.description && <p className="item-desc">{exp.description}</p>}
                    </div>
                    <div className="item-card-actions">
                      <strong className="expense-amount">${Number(exp.amount).toLocaleString()}</strong>
                      <button className="btn btn-ghost btn-sm" onClick={() => setExpModal(exp)}>Edit</button>
                      <button className="btn btn-sm" style={{ color: 'var(--color-danger)', background: 'var(--color-danger-light)' }} onClick={() => handleDeleteExpense(exp.id)}>Delete</button>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        )}

        {activeTab === 'checklist' && (
          <div className="section-panel">
            <h2 className="section-title" style={{ marginBottom: '16px' }}>Packing checklist</h2>
            <form onSubmit={handleAddChecklistItem} className="checklist-add-form">
              <input
                className="form-input"
                value={newChecklistItem}
                onChange={(e) => setNewChecklistItem(e.target.value)}
                placeholder="Add an item (e.g. passport, charger...)"
              />
              <button type="submit" className="btn btn-primary" disabled={!newChecklistItem.trim()}>
                Add
              </button>
            </form>
            {checklist.length === 0 ? (
              <div className="empty-state"><h3>Checklist is empty</h3><p>Add items you need to prepare before your trip.</p></div>
            ) : (
              <div className="card" style={{ marginTop: '16px' }}>
                <p className="checklist-progress">
                  {checklist.filter((c) => c.isCompleted).length} of {checklist.length} completed
                </p>
                <div className="divider" />
                {checklist.map((item) => (
                  <ChecklistItem
                    key={item.id}
                    item={item}
                    onToggle={handleToggleChecklistItem}
                    onDelete={handleDeleteChecklistItem}
                  />
                ))}
              </div>
            )}
          </div>
        )}

        {activeTab === 'sharing' && (
          <div className="section-panel">
            <h2 className="section-title" style={{ marginBottom: '8px' }}>Share this trip</h2>
            <p className="sharing-desc">Generate a link to share this trip plan with others. Choose view-only or edit access.</p>
            <div className="card" style={{ marginTop: '16px' }}>
              <SharedLinks tripId={tripId} />
            </div>
          </div>
        )}
      </div>

      {editTripModal && (
        <div className="modal-overlay" onClick={closeModal}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>Edit trip</h2>
              <button className="modal-close" onClick={closeModal}>×</button>
            </div>
            {formError && <div className="alert alert-error">{formError}</div>}
            <TripForm initialData={currentTrip} onSubmit={handleUpdateTrip} onCancel={closeModal} loading={formLoading} />
          </div>
        </div>
      )}

      {destModal !== null && (
        <div className="modal-overlay" onClick={closeModal}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>{destModal.id ? 'Edit destination' : 'Add destination'}</h2>
              <button className="modal-close" onClick={closeModal}>×</button>
            </div>
            {formError && <div className="alert alert-error">{formError}</div>}
            <DestinationForm initialData={destModal.id ? destModal : null} onSubmit={handleSaveDestination} onCancel={closeModal} loading={formLoading} />
          </div>
        </div>
      )}

      {actModal !== null && (
        <div className="modal-overlay" onClick={closeModal}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>{actModal.id ? 'Edit activity' : 'Add activity'}</h2>
              <button className="modal-close" onClick={closeModal}>×</button>
            </div>
            {formError && <div className="alert alert-error">{formError}</div>}
            <ActivityForm initialData={actModal.id ? actModal : null} onSubmit={handleSaveActivity} onCancel={closeModal} loading={formLoading} />
          </div>
        </div>
      )}

      {expModal !== null && (
        <div className="modal-overlay" onClick={closeModal}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>{expModal.id ? 'Edit expense' : 'Add expense'}</h2>
              <button className="modal-close" onClick={closeModal}>×</button>
            </div>
            {formError && <div className="alert alert-error">{formError}</div>}
            <ExpenseForm initialData={expModal.id ? expModal : null} onSubmit={handleSaveExpense} onCancel={closeModal} loading={formLoading} />
          </div>
        </div>
      )}
    </div>
  );
}