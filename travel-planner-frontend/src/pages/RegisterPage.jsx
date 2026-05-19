import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import authService from '../services/authService';
import '../css/AuthPage.css';

function validate(form) {
  const errors = {};
  if (!form.firstName.trim()) errors.firstName = 'First name is required.';
  if (!form.lastName.trim()) errors.lastName = 'Last name is required.';
  if (!form.email) errors.email = 'Email is required.';
  else if (!/\S+@\S+\.\S+/.test(form.email)) errors.email = 'Enter a valid email.';
  if (!form.password) errors.password = 'Password is required.';
  else if (form.password.length < 6) errors.password = 'Password must be at least 6 characters.';
  return errors;
}

export default function RegisterPage() {
  const [form, setForm] = useState({ firstName: '', lastName: '', email: '', password: '' });
  const [errors, setErrors] = useState({});
  const [serverError, setServerError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  function handleChange(e) {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
    if (errors[name]) setErrors((prev) => ({ ...prev, [name]: '' }));
  }

  async function handleSubmit(e) {
    e.preventDefault();
    const errs = validate(form);
    if (Object.keys(errs).length > 0) { setErrors(errs); return; }

    setLoading(true);
    setServerError('');
    try {
      // response je AuthResponseDto: { userId, token, email, firstName, lastName, role }
      const response = await authService.register(form);
      login(response.token, response);
      navigate('/trips');
    } catch (err) {
      setServerError(err.message || 'Registration failed.');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="auth-page">
      <div className="auth-card">
        <div className="auth-header">
          <h1 className="auth-brand">TravelPlanner<span>.</span></h1>
          <p className="auth-subtitle">Create your account</p>
        </div>

        {serverError && <div className="alert alert-error">{serverError}</div>}

        <form onSubmit={handleSubmit} noValidate>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
            <div className="form-group">
              <label className="form-label">First Name</label>
              <input
                className={`form-input ${errors.firstName ? 'error' : ''}`}
                name="firstName"
                value={form.firstName}
                onChange={handleChange}
                placeholder="John"
              />
              {errors.firstName && <span className="form-error">{errors.firstName}</span>}
            </div>
            <div className="form-group">
              <label className="form-label">Last Name</label>
              <input
                className={`form-input ${errors.lastName ? 'error' : ''}`}
                name="lastName"
                value={form.lastName}
                onChange={handleChange}
                placeholder="Doe"
              />
              {errors.lastName && <span className="form-error">{errors.lastName}</span>}
            </div>

            <div className="form-group">
              <label className="form-label">Email</label>
              <input
                type="email"
                className={`form-input ${errors.email ? 'error' : ''}`}
                name="email"
                value={form.email}
                onChange={handleChange}
                placeholder="you@example.com"
                autoComplete="email"
              />
              {errors.email && <span className="form-error">{errors.email}</span>}
            </div>

            <div className="form-group">
              <label className="form-label">Password</label>
              <input
                type="password"
                className={`form-input ${errors.password ? 'error' : ''}`}
                name="password"
                value={form.password}
                onChange={handleChange}
                placeholder="At least 6 characters"
                autoComplete="new-password"
              />
              {errors.password && <span className="form-error">{errors.password}</span>}
            </div>

            <button
              type="submit"
              className="btn btn-primary"
              style={{ width: '100%', justifyContent: 'center', marginTop: '4px' }}
              disabled={loading}
            >
              {loading ? 'Creating account...' : 'Create account'}
            </button>
          </div>
        </form>

        <p className="auth-footer">
          Already have an account? <Link to="/login">Sign in</Link>
        </p>
      </div>
    </div>
  );
}
