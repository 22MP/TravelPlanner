import './ChecklistItem.css';

export default function ChecklistItem({ item, onToggle, onDelete }) {
  return (
    <div className={`checklist-item ${item.isCompleted ? 'completed' : ''}`}>
      <button
        className="checklist-checkbox"
        onClick={() => onToggle(item)}
        aria-label={item.isCompleted ? 'Mark as incomplete' : 'Mark as complete'}
      >
        {item.isCompleted && (
          <svg width="12" height="12" viewBox="0 0 12 12" fill="none">
            <path d="M2 6l3 3 5-5" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
          </svg>
        )}
      </button>
      <span className="checklist-label">{item.name || item.title}</span>
      <button
        className="checklist-delete"
        onClick={() => onDelete(item.id)}
        aria-label="Delete item"
      >
        ×
      </button>
    </div>
  );
}
