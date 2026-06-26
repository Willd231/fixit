import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { api } from '../api';

const UsersPage = () => {
  const q = useQuery({ queryKey: ['users'], queryFn: () => api.getUsers() });
  return (
    <div>
      <h2>Technicians</h2>
      <div className="container">
        {q.isLoading && <div>Loading...</div>}
        {q.isError && <div>Error</div>}
        {q.data && (
          <table className="table">
            <thead><tr><th>Name</th><th>Email</th><th>Role</th><th>Team</th></tr></thead>
            <tbody>
              {q.data.map((u:any)=>(<tr key={u.id}><td>{u.name}</td><td>{u.email}</td><td>{u.role}</td><td>{u.teamId ?? '—'}</td></tr>))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
};

export default UsersPage;
