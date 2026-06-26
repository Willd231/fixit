import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { api } from '../api';

const ReportsPage = () => {
  const statsQ = useQuery({ queryKey: ['dashboardStats'], queryFn: () => api.getDashboardStats() });
  return (
    <div>
      <h2>Reports</h2>
      <div className="container">
        {statsQ.isLoading && <div>Loading...</div>}
        {statsQ.data && (
          <div>
            <div>Open Tickets: {statsQ.data?.totalOpen}</div>
            <div>Critical: {statsQ.data?.critical}</div>
            <div>Awaiting Assignment: {statsQ.data?.awaitingAssignment}</div>
          </div>
        )}
      </div>
    </div>
  );
};

export default ReportsPage;
