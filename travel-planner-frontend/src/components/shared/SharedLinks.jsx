import { useState, useEffect } from 'react';
import { QRCodeSVG } from 'qrcode.react';
import sharedLinkService from '../../services/sharedLinkService';
import '../../css/SharedLinks.css';

const ACCESS_TYPES = ['view', 'edit'];

export default function SharedLinks({ tripId }) {
  const [links, setLinks] = useState([]);
  const [accessType, setAccessType] = useState('view');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [copied, setCopied] = useState(null);
  const [showQr, setShowQr] = useState(null);

  useEffect(() => {
    sharedLinkService.getAllByTrip(tripId).then(setLinks).catch(() => {});
  }, [tripId]);

  async function handleCreate() {
    setLoading(true);
    setError('');
    try {
      const link = await sharedLinkService.create(tripId, { accessType });
      setLinks((prev) => [...prev, link]);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  async function handleDelete(id) {
    if (!window.confirm('Delete this shared link?')) return;
    try {
      await sharedLinkService.delete(tripId, id);
      setLinks((prev) => prev.filter((l) => l.id !== id));
      if (showQr === id) setShowQr(null);
    } catch (err) {
      setError(err.message);
    }
  }

  function handleCopy(token) {
    const url = `${window.location.origin}/shared/${token}`;
    navigator.clipboard.writeText(url).then(() => {
      setCopied(token);
      setTimeout(() => setCopied(null), 2000);
    });
  }

  function getUrl(token) {
    return `${window.location.origin}/shared/${token}`;
  }

  function formatDate(dateStr) {
    if (!dateStr) return '';
    return new Date(dateStr).toLocaleDateString('en-GB', {
      day: 'numeric', month: 'short', year: 'numeric',
    });
  }

  return (
    <div className="shared-links">
      <div className="shared-links-create">
        <select
          className="form-select"
          value={accessType}
          onChange={(e) => setAccessType(e.target.value)}
          style={{ width: 'auto' }}
        >
          {ACCESS_TYPES.map((t) => (
            <option key={t} value={t}>
              {t.charAt(0).toUpperCase() + t.slice(1)} access
            </option>
          ))}
        </select>
        <button className="btn btn-primary btn-sm" onClick={handleCreate} disabled={loading}>
          {loading ? 'Generating...' : 'Generate link'}
        </button>
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      {links.length === 0 ? (
        <p className="shared-empty">No shared links yet.</p>
      ) : (
        <div className="shared-links-list">
          {links.map((link) => (
            <div key={link.id} className="shared-link-row">
              <div className="shared-link-info">
                <span className="shared-link-type">
                  {typeof link.accessType === 'number'
                    ? link.accessType === 0 ? 'VIEW' : 'EDIT'
                    : link.accessType?.toUpperCase()} access
                </span>
                {link.expiresAt && (
                  <span className="shared-link-expiry">
                    Expires: {formatDate(link.expiresAt)}
                  </span>
                )}
                <code className="shared-link-token">{link.token}</code>
              </div>

              {showQr === link.id && (
                <div className="shared-link-qr">
                  <QRCodeSVG value={getUrl(link.token)} size={150} />
                  <p className="qr-hint">Scan to open shared trip</p>
                </div>
              )}

              <div className="shared-link-actions">
                <button
                  className="btn btn-ghost btn-sm"
                  onClick={() => handleCopy(link.token)}
                >
                  {copied === link.token ? 'Copied!' : 'Copy link'}
                </button>
                <button
                  className="btn btn-ghost btn-sm"
                  onClick={() => setShowQr(showQr === link.id ? null : link.id)}
                >
                  {showQr === link.id ? 'Hide QR' : 'Show QR'}
                </button>
                <button
                  className="btn btn-sm"
                  style={{ color: 'var(--color-danger)', background: 'var(--color-danger-light)' }}
                  onClick={() => handleDelete(link.id)}
                >
                  Delete
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}