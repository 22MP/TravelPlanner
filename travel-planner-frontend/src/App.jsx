import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { TripProvider } from './context/TripContext';
import ProtectedRoute from './components/common/ProtectedRoute';
import Navbar from './components/common/Navbar';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import TripsPage from './pages/TripsPage';
import TripDetailPage from './pages/TripDetailPage';
import AdminPage from './pages/AdminPage';
import SharedTripPage from './pages/SharedTripPage';

export default function App() {
    return (
        <BrowserRouter>
            <AuthProvider>
                <TripProvider>
                    <Navbar />
                    <Routes>
                        <Route path="/login" element={<LoginPage />} />
                        <Route path="/register" element={<RegisterPage />} />
                        <Route path="/shared/:token" element={<SharedTripPage />} />
                        <Route
                            path="/trips"
                            element={
                                <ProtectedRoute>
                                    <TripsPage />
                                </ProtectedRoute>
                            }
                        />
                        <Route
                            path="/trips/:id"
                            element={
                                <ProtectedRoute>
                                    <TripDetailPage />
                                </ProtectedRoute>
                            }
                        />
                        <Route
                            path="/admin"
                            element={
                                <ProtectedRoute adminOnly>
                                    <AdminPage />
                                </ProtectedRoute>
                            }
                        />
                        <Route path="/" element={<Navigate to="/trips" replace />} />
                        <Route path="*" element={<Navigate to="/trips" replace />} />
                    </Routes>
                </TripProvider>
            </AuthProvider>
        </BrowserRouter>
    );
}