import { useState, useEffect } from 'react';
import authService from '../services/authService';
import { User } from '../models/User';
import './AdminPage.css';

export default function AdminPage() {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    authService
      .getAllUsers()
      .then((data) => setUsers(data.map((u) => new User(u))))
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  async function handleDelete(id) {
    if (!window.confirm('Delete this user? All their data will be removed.')) return;
    try {
      await authService.deleteUser(id);
      setUsers((prev) => prev.filter((u) => u.id !== id));
    } catch (err) {
      setError(err.message);
    }
  }

  return (
    <div className="admin-page">
      <div className="page-header">
        <h1>User management</h1>
        <p>View and manage all registered users.</p>
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      {loading ? (
        <div className="loading-center"><div className="spinner" /></div>
      ) : users.length === 0 ? (
        <div className="empty-state"><h3>No users found</h3></div>
      ) : (
        <div className="admin-table-wrapper card">
          <table className="admin-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Email</th>
                <th>Role</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {users.map((user) => (
                <tr key={user.id}>
                  <td className="td-muted">{user.id}</td>
                  {/* user.name je firstName + lastName iz User konstruktora */}
                  <td>{user.name}</td>
                  <td className="td-muted">{user.email}</td>
                  <td>
                    <span className={`badge ${user.role === 'admin' ? 'badge-reserved' : 'badge-planned'}`}>
                      {user.role}
                    </span>
                  </td>
                  <td>
                    <button
                      className="btn btn-sm"
                      style={{ color: 'var(--color-danger)', background: 'var(--color-danger-light)' }}
                      onClick={() => handleDelete(user.id)}
                      disabled={user.role === 'admin'}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
