import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import './Navbar.css';

export default function Navbar() {
  const { user, isAuthenticated, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  function handleLogout() {
    logout();
    navigate('/login');
  }

  if (!isAuthenticated) return null;

  // user.name je firstName + lastName, postavljeno u User konstruktoru
  const displayName = user?.name || user?.email || '';

  return (
    <header className="navbar">
      <div className="navbar-inner">
        <Link to="/trips" className="navbar-brand">
          TravelPlanner
        </Link>

        <nav className="navbar-nav">
          <Link
            to="/trips"
            className={`nav-link ${location.pathname === '/trips' ? 'active' : ''}`}
          >
            My Trips
          </Link>
          {user?.role === 'admin' && (
            <Link
              to="/admin"
              className={`nav-link ${location.pathname === '/admin' ? 'active' : ''}`}
            >
              Admin
            </Link>
          )}
        </nav>

        <div className="navbar-user">
          <span className="navbar-username">{displayName}</span>
          <button className="btn btn-ghost btn-sm" onClick={handleLogout}>
            Sign out
          </button>
        </div>
      </div>
    </header>
  );
}
