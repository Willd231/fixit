import React from 'react';
import { Search } from 'lucide-react';
import { useAuth } from '../context/AuthContext';

const Header = () => {
  const { user } = useAuth();
  return (
    <header className="header">
      <div className="left">
        <input className="search" placeholder="Search tickets, requester, assignee..." />
      </div>
      <div className="top-actions">
        <button title="Notifications">🔔</button>
        <div style={{display:'flex',flexDirection:'column',alignItems:'flex-end'}}>
          <div style={{fontSize:12,color:'var(--muted)'}}>Signed in as</div>
          <div style={{fontWeight:600}}>{user?.name ?? 'Guest'}</div>
        </div>
      </div>
    </header>
  );
};

export default Header;
