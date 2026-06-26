import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { api } from '../api';
import StatCard from '../components/StatCard';
import DataTable from '../components/DataTable';
import StatusBadge from '../components/StatusBadge';

const DashboardPage = () => {
  const statsQ = useQuery({ queryKey: ['dashboardStats'], queryFn: () => api.getDashboardStats() });
  const recentQ = useQuery({ queryKey: ['recentTickets'], queryFn: () => api.getTickets({ page: 1, pageSize: 5 }) });

  return (
    <div>
      <h2>Dashboard</h2>
      <div className="card-grid">
        <StatCard title="Open Tickets" value={statsQ.data?.totalOpen ?? '—'} />
        <StatCard title="Critical" value={statsQ.data?.critical ?? '—'} />
        <StatCard title="Awaiting Assignment" value={statsQ.data?.awaitingAssignment ?? '—'} />
        <StatCard title="Resolved This Week" value={statsQ.data?.resolvedThisWeek ?? '—'} />
      </div>

      <section style={{marginTop:16}}>
        <h3 style={{marginBottom:8}}>Recently Submitted</h3>
        <div className="container">
          {recentQ.isLoading && <div>Loading...</div>}
          {recentQ.isError && <div>Error loading recent tickets</div>}
          {recentQ.data && (
            <DataTable
              columns={[
                { key: 'ticketNumber', label: 'Ticket' },
                { key: 'title', label: 'Title' },
                { key: 'status', label: 'Status', render: (r)=> <StatusBadge status={r.status} /> },
                { key: 'priority', label: 'Priority' },
                { key: 'createdAt', label: 'Created' }
              ]}
              data={recentQ.data.items}
            />
          )}
        </div>
      </section>
    </div>
  );
};

export default DashboardPage;
