import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { api } from '../api';

const TeamsPage = () => {
  const q = useQuery({ queryKey: ['teams'], queryFn: () => api.getTeams() });
  return (
    <div>
      <h2>Teams</h2>
      <div className="container">
        {q.isLoading && <div>Loading teams...</div>}
        {q.isError && <div>Error</div>}
        {q.data && (
          <ul>
            {q.data.map((t:any) => (
              <li key={t.id} style={{padding:8,borderBottom:'1px solid #f1f5f9'}}>
                <div style={{fontWeight:600}}>{t.name}</div>
                <div style={{color:'var(--muted)'}}>{t.description}</div>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
};

export default TeamsPage;
