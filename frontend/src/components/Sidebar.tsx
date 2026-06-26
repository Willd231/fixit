import React from 'react';
import { NavLink } from 'react-router-dom';
import { Wrench } from 'lucide-react';

const Sidebar = () => {
  return (
    <aside className="sidebar">
      <div className="brand">
        <Wrench />
        <div>
          <div style={{fontSize:18}}>FixIt</div>
          <div style={{fontSize:12,color:'var(--muted)'}}>Issue Tracking 2025</div>
        </div>
      </div>

      <nav>
        <NavLink to="/" end className={({isActive})=>"nav-link" + (isActive? ' active':'')}>Dashboard</NavLink>
        <NavLink to="/tickets" className={({isActive})=>"nav-link" + (isActive? ' active':'')}>Tickets</NavLink>
        <NavLink to="/teams" className={({isActive})=>"nav-link" + (isActive? ' active':'')}>Teams</NavLink>
        <NavLink to="/users" className={({isActive})=>"nav-link" + (isActive? ' active':'')}>Technicians</NavLink>
        <NavLink to="/reports" className={({isActive})=>"nav-link" + (isActive? ' active':'')}>Reports</NavLink>
      </nav>
    </aside>
  );
};

export default Sidebar;
