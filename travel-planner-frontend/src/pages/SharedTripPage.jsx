import { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import sharedLinkService from '../services/sharedLinkService';
import tripService from '../services/tripService';
import destinationService from '../services/destinationService';
import activityService from '../services/activityService';
import checklistService from '../services/checklistService';
import expenseService from '../services/expenseService';
import { Activity } from '../models/Activity';
import { Expense } from '../models/Expense';
import './SharedTripPage.css';

function formatDate(dateStr) {
  if (!dateStr) return '';
  return new Date(dateStr).toLocaleDateString('en-GB', {
    day: 'numeric', month: 'short', year: 'numeric',
  });
}

function groupByDate(items) {
  return items.reduce((acc, item) => {
    const date = item.date?.split('T')[0] || 'No date';
    if (!acc[date]) acc[date] = [];
    acc[date].push(item);
    return acc;
  }, {});
}

const TABS = ['overview', 'destinations', 'activities', 'expenses', 'checklist'];

export default function SharedTripPage() {
  const { token } = useParams();

  const [linkData, setLinkData] = useState(null);
  const [trip, setTrip] = useState(null);
  const [destinations, setDestinations] = useState([]);
  const [activities, setActivities] = useState([]);
  const [expenses, setExpenses] = useState([]);
  const [checklist, setChecklist] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [activeTab, setActiveTab] = useState('overview');

  useEffect(() => {
    async function load() {
      try {
        // Korak 1: dohvati link info (tripId, accessType)
        const link = await sharedLinkService.getByToken(token);
        if (!link || link.id === 0) {
          setError('Invalid or expired link.');
          return;
        }
        setLinkData(link);

        const tripId = link.tripId;

        // Korak 2: dohvati sve podatke o tripu koristeći X-Share-Token header
        // httpClient automatski dodaje JWT, ali za shared pristup koristimo token u headeru
        // Pozivamo direktno sa tokenom
        const [tripData, dests, acts, exps, checks] = await Promise.all([
          tripService.getById(tripId),
          destinationService.getAllByTrip(tripId),
          activityService.getAllByTrip(tripId),
          expenseService.getAllByTrip(tripId),
          checklistService.getAllByTrip(tripId),
        ]);

        setTrip(tripData);
        setDestinations(dests);
        setActivities(acts.map((a) => new Activity(a)));
        setExpenses(exps.map((e) => new Expense(e)));
        setChecklist(checks);
      } catch (err) {
        setError(err.message || 'Invalid or expired link.');
      } finally {
        setLoading(false);
      }
    }
    load();
  }, [token]);

  if (loading) return <div className="loading-center"><div className="spinner" /></div>;

  if (error || !trip) {
    return (
      <div className="shared-error-page">
        <h1>Link unavailable</h1>
        <p>{error || 'Trip not found.'}</p>
      </div>
    );
  }

  const totalExpenses = expenses.reduce((sum, e) => sum + Number(e.amount || 0), 0);
  const remaining = trip.budget ? Number(trip.budget) - totalExpenses : 0;
  const activitiesByDate = groupByDate(activities);
  const accessLabel = typeof linkData?.accessType === 'number'
    ? linkData.accessType === 0 ? 'VIEW' : 'EDIT'
    : linkData?.accessType?.toUpperCase() || 'VIEW';

  return (
    <div className="shared-page">
      <div className="shared-header">
        <div className="auth-brand" style={{ marginBottom: '8px' }}>
          TravelPlanner<span style={{ color: 'var(--color-accent)' }}>.</span>
        </div>
        <h1>{trip.name}</h1>
        {trip.description && <p className="shared-desc">{trip.description}</p>}
        <div className="detail-meta" style={{ marginTop: '8px' }}>
          {trip.startDate && (
            <span>{formatDate(trip.startDate)} — {formatDate(trip.endDate)}</span>
          )}
          {trip.budget > 0 && (
            <span>Budget: <strong>${Number(trip.budget).toLocaleString()}</strong></span>
          )}
        </div>
        <span className="badge badge-planned" style={{ marginTop: '8px', display: 'inline-block' }}>
          {accessLabel} access
        </span>
      </div>

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
                  <span className="stat-value">
                    {checklist.filter((c) => c.isCompleted).length}/{checklist.length}
                  </span>
                  <span className="stat-label">Checklist</span>
                </div>
              </div>
            </div>
            {trip.budget > 0 && (
              <div className="card">
                <h3 className="section-title">Budget</h3>
                <div className="divider" />
                <div className="budget-bar-container">
                  <div className="budget-bar">
                    <div
                      className="budget-bar-fill"
                      style={{
                        width: `${Math.min((totalExpenses / trip.budget) * 100, 100)}%`,
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
            {trip.notes && (
              <div className="card">
                <h3 className="section-title">Notes</h3>
                <div className="divider" />
                <p className="notes-text">{trip.notes}</p>
              </div>
            )}
          </div>
        )}

        {activeTab === 'destinations' && (
          <div className="section-panel">
            <h2 className="section-title">Destinations</h2>
            {destinations.length === 0 ? (
              <div className="empty-state"><h3>No destinations added</h3></div>
            ) : (
              <div className="items-list">
                {destinations.map((dest) => (
                  <div key={dest.id} className="item-card">
                    <div className="item-card-body">
                      <h4>{dest.name}</h4>
                      {dest.location && <p className="item-meta">{dest.location}</p>}
                      {dest.arrivalDate && (
                        <p className="item-meta">
                          {formatDate(dest.arrivalDate)} — {formatDate(dest.departureDate)}
                        </p>
                      )}
                      {dest.description && <p className="item-desc">{dest.description}</p>}
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        )}

        {activeTab === 'activities' && (
          <div className="section-panel">
            <h2 className="section-title">Activities</h2>
            {activities.length === 0 ? (
              <div className="empty-state"><h3>No activities planned</h3></div>
            ) : (
              <div className="activities-by-date">
                {Object.entries(activitiesByDate)
                  .sort(([a], [b]) => a.localeCompare(b))
                  .map(([date, acts]) => (
                    <div key={date} className="date-group">
                      <h4 className="date-group-label">
                        {date === 'No date' ? date : formatDate(date)}
                      </h4>
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
                              {act.estimatedCost > 0 && (
                                <p className="item-meta">Est. cost: ${act.estimatedCost}</p>
                              )}
                              {act.description && <p className="item-desc">{act.description}</p>}
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
            <h2 className="section-title">Expenses</h2>
            {expenses.length === 0 ? (
              <div className="empty-state"><h3>No expenses logged</h3></div>
            ) : (
              <div className="items-list">
                {expenses.map((exp) => (
                  <div key={exp.id} className="item-card">
                    <div className="item-card-body">
                      <h4>{exp.name}</h4>
                      <div style={{ display: 'flex', gap: '10px', alignItems: 'center' }}>
                        <span className="badge" style={{
                          background: 'var(--color-surface-alt)',
                          color: 'var(--color-text-secondary)',
                          border: '1px solid var(--color-border)'
                        }}>{exp.category}</span>
                        {exp.date && <span className="item-meta">{formatDate(exp.date)}</span>}
                      </div>
                    </div>
                    <div className="item-card-actions">
                      <strong className="expense-amount">${Number(exp.amount).toLocaleString()}</strong>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        )}

        {activeTab === 'checklist' && (
          <div className="section-panel">
            <h2 className="section-title">Packing checklist</h2>
            {checklist.length === 0 ? (
              <div className="empty-state"><h3>Checklist is empty</h3></div>
            ) : (
              <div className="card" style={{ marginTop: '16px' }}>
                <p className="checklist-progress">
                  {checklist.filter((c) => c.isCompleted).length} of {checklist.length} completed
                </p>
                <div className="divider" />
                {checklist.map((item) => (
                  <div key={item.id} className={`checklist-item ${item.isCompleted ? 'completed' : ''}`}>
                    <span className="checklist-checkbox">
                      {item.isCompleted && (
                        <svg width="12" height="12" viewBox="0 0 12 12" fill="none">
                          <path d="M2 6l3 3 5-5" stroke="currentColor" strokeWidth="2"
                            strokeLinecap="round" strokeLinejoin="round" />
                        </svg>
                      )}
                    </span>
                    <span className="checklist-label">{item.name}</span>
                  </div>
                ))}
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
}