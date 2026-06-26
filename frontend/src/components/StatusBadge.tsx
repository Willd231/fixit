import React from 'react';

const colorFor = (status: string) => {
  switch (status) {
    case 'New': return '#0b6cff';
    case 'Open': return '#06b6d4';
    case 'In Progress': return '#f59e0b';
    case 'Pending': return '#6366f1';
    case 'Resolved': return '#10b981';
    case 'Closed': return '#6b7280';
    default: return '#94a3b8';
  }
};

const StatusBadge = ({ status }: { status: string }) => (
  <span style={{background: colorFor(status), color: 'white', padding: '4px 8px', borderRadius: 999, fontSize:12}}>{status}</span>
);

export default StatusBadge;
