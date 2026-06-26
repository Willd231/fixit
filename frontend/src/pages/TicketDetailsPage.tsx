import React, { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '../api';
import StatusBadge from '../components/StatusBadge';

const TicketDetailsPage = () => {
  const { id } = useParams();
  const ticketId = Number(id);
  const qc = useQueryClient();
  const nav = useNavigate();
  const q = useQuery({ queryKey: ['ticket', ticketId], queryFn: () => api.getTicket(ticketId), enabled: !!ticketId });
  const [comment, setComment] = useState('');
  const updateM = useMutation({ mutationFn: (patch: any) => api.updateTicket(ticketId, patch), onSuccess: () => qc.invalidateQueries({ queryKey: ['ticket', ticketId] }) });
  const delM = useMutation({ mutationFn: () => api.deleteTicket(ticketId), onSuccess: ()=> { qc.invalidateQueries({ queryKey: ['tickets'] }); nav('/tickets'); } });

  if (!ticketId) return <div>Invalid ticket</div>;
  if (q.isLoading) return <div>Loading ticket...</div>;
  if (q.isError) return <div>Error loading ticket</div>;

  const t = q.data as any;

  return (
    <div>
      <div style={{display:'flex',justifyContent:'space-between',alignItems:'center'}}>
        <div>
          <h2>{t.ticketNumber} — {t.title}</h2>
          <div style={{marginTop:6}}>Requester: {t.requesterName} • {t.requesterEmail}</div>
        </div>
        <div style={{display:'flex',gap:8,alignItems:'center'}}>
          <StatusBadge status={t.status} />
          <button onClick={()=>updateM.mutate({ status: 'Resolved', resolvedAt: new Date().toISOString() })}>Mark Resolved</button>
          <button onClick={()=>updateM.mutate({ status: 'Open' })}>Reopen</button>
          <button onClick={()=>delM.mutate()} style={{background:'#ef4444'}}>Delete</button>
        </div>
      </div>

      <div className="container" style={{marginTop:12}}>
        <h3>Description</h3>
        <p>{t.description}</p>

        <h4>Activity</h4>
        <ul>
          {t.activity?.map((a: any)=> (<li key={a.id}><strong>{a.type}</strong>: {a.message} <span style={{color:'var(--muted)'}}> — {new Date(a.createdAt).toLocaleString()}</span></li>))}
        </ul>

        <h4>Comments</h4>
        <ul>
          {t.comments?.map((c: any)=> (<li key={c.id}><strong>{c.authorName}</strong>: {c.content} <span style={{color:'var(--muted)'}}> — {new Date(c.createdAt).toLocaleString()}</span></li>))}
        </ul>

        <div style={{marginTop:8}}>
          <textarea value={comment} onChange={(e)=>setComment(e.target.value)} rows={3} style={{width:'100%'}} />
          <div style={{display:'flex',gap:8,marginTop:8}}>
            <button onClick={async ()=>{ if(!comment.trim()) return; await api.addComment(ticketId, 4, 'Admin User', comment); setComment(''); qc.invalidateQueries({ queryKey: ['ticket', ticketId] }); }}>Add Comment</button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default TicketDetailsPage;
