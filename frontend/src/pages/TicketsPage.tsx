import React, { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { api } from '../api';
import DataTable from '../components/DataTable';
import StatusBadge from '../components/StatusBadge';
import PriorityBadge from '../components/PriorityBadge';
import { Link } from 'react-router-dom';

const TicketsPage = () => {
  const [search, setSearch] = useState('');
  const q = useQuery({ queryKey: ['tickets', search], queryFn: () => api.getTickets({ search, page:1, pageSize:20 }) });

  return (
    <div>
      <div style={{display:'flex',justifyContent:'space-between',alignItems:'center'}}>
        <h2>Tickets</h2>
        <div>
          <Link to="/tickets/new"><button>Create Ticket</button></Link>
        </div>
      </div>

      <div style={{marginTop:12,marginBottom:12}}>
        <input placeholder="Search tickets" value={search} onChange={(e)=>setSearch(e.target.value)} style={{padding:8,borderRadius:8,border:'1px solid #e6edf8',width:'100%'}} />
      </div>

      <div className="container">
        {q.isLoading && <div>Loading tickets...</div>}
        {q.isError && <div>Error loading tickets</div>}
        {q.data && (
          <DataTable
            columns={[
              { key: 'ticketNumber', label: 'Ticket' },
              { key: 'title', label: 'Title', render: (r)=> <Link to={`/tickets/${r.id}`}>{r.title}</Link> },
              { key: 'requesterName', label: 'Requester' },
              { key: 'category', label: 'Category' },
              { key: 'priority', label: 'Priority', render: (r)=> <PriorityBadge priority={r.priority} /> },
              { key: 'status', label: 'Status', render: (r)=> <StatusBadge status={r.status} /> },
              { key: 'assignedTeamName', label: 'Team' },
              { key: 'assignedUserName', label: 'Technician' },
              { key: 'createdAt', label: 'Created' }
            ]}
            data={q.data.items}
          />
        )}
      </div>
    </div>
  );
};

export default TicketsPage;
