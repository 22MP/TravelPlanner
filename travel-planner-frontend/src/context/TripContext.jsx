import { createContext, useContext, useReducer } from 'react';

const TripContext = createContext();

const initialState = {
  trips: [],
  currentTrip: null,
  destinations: [],
  activities: [],
  expenses: [],
  checklist: [],
};

function tripReducer(state, action) {
  switch (action.type) {
    case 'SET_TRIPS':
      return { ...state, trips: action.payload };
    case 'ADD_TRIP':
      return { ...state, trips: [action.payload, ...state.trips] };
    case 'UPDATE_TRIP':
      return {
        ...state,
        trips: state.trips.map((t) => (t.id === action.payload.id ? action.payload : t)),
        currentTrip: state.currentTrip?.id === action.payload.id ? action.payload : state.currentTrip,
      };
    case 'DELETE_TRIP':
      return { ...state, trips: state.trips.filter((t) => t.id !== action.payload) };
    case 'SET_CURRENT_TRIP':
      return { ...state, currentTrip: action.payload };
    case 'SET_DESTINATIONS':
      return { ...state, destinations: action.payload };
    case 'ADD_DESTINATION':
      return { ...state, destinations: [...state.destinations, action.payload] };
    case 'UPDATE_DESTINATION':
      return {
        ...state,
        destinations: state.destinations.map((d) => (d.id === action.payload.id ? action.payload : d)),
      };
    case 'DELETE_DESTINATION':
      return { ...state, destinations: state.destinations.filter((d) => d.id !== action.payload) };
    case 'SET_ACTIVITIES':
      return { ...state, activities: action.payload };
    case 'ADD_ACTIVITY':
      return { ...state, activities: [...state.activities, action.payload] };
    case 'UPDATE_ACTIVITY':
      return {
        ...state,
        activities: state.activities.map((a) => (a.id === action.payload.id ? action.payload : a)),
      };
    case 'DELETE_ACTIVITY':
      return { ...state, activities: state.activities.filter((a) => a.id !== action.payload) };
    case 'SET_EXPENSES':
      return { ...state, expenses: action.payload };
    case 'ADD_EXPENSE':
      return { ...state, expenses: [...state.expenses, action.payload] };
    case 'UPDATE_EXPENSE':
      return {
        ...state,
        expenses: state.expenses.map((e) => (e.id === action.payload.id ? action.payload : e)),
      };
    case 'DELETE_EXPENSE':
      return { ...state, expenses: state.expenses.filter((e) => e.id !== action.payload) };
    case 'SET_CHECKLIST':
      return { ...state, checklist: action.payload };
    case 'ADD_CHECKLIST_ITEM':
      return { ...state, checklist: [...state.checklist, action.payload] };
    case 'UPDATE_CHECKLIST_ITEM':
      return {
        ...state,
        checklist: state.checklist.map((c) => (c.id === action.payload.id ? action.payload : c)),
      };
    case 'DELETE_CHECKLIST_ITEM':
      return { ...state, checklist: state.checklist.filter((c) => c.id !== action.payload) };
    default:
      return state;
  }
}

export function TripProvider({ children }) {
  const [state, dispatch] = useReducer(tripReducer, initialState);

  return (
    <TripContext.Provider value={{ ...state, dispatch }}>
      {children}
    </TripContext.Provider>
  );
}

export function useTripContext() {
  const context = useContext(TripContext);
  if (!context) {
    throw new Error('useTripContext must be used within TripProvider');
  }
  return context;
}
