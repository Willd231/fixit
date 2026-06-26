import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import MainLayout from './layouts/MainLayout';
import DashboardPage from './pages/DashboardPage';
import TicketsPage from './pages/TicketsPage';
import TicketCreatePage from './pages/TicketCreatePage';
import TicketDetailsPage from './pages/TicketDetailsPage';
import TeamsPage from './pages/TeamsPage';
import UsersPage from './pages/UsersPage';
import ReportsPage from './pages/ReportsPage';
import LoginPage from './pages/LoginPage';
import { AuthProvider } from './context/AuthContext';

function App() {
  return (
    <AuthProvider>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/" element={<MainLayout />}>
          <Route index element={<DashboardPage />} />
          <Route path="tickets" element={<TicketsPage />} />
          <Route path="tickets/new" element={<TicketCreatePage />} />
          <Route path="tickets/:id" element={<TicketDetailsPage />} />
          <Route path="teams" element={<TeamsPage />} />
          <Route path="users" element={<UsersPage />} />
          <Route path="reports" element={<ReportsPage />} />
        </Route>
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </AuthProvider>
  );
}

export default App;
