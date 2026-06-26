import React from 'react';

const colorFor = (p: string) => {
  switch (p) {
    case 'Low': return '#10b981';
    case 'Medium': return '#06b6d4';
    case 'High': return '#f97316';
    case 'Critical': return '#ef4444';
    default: return '#94a3b8';
  }
};

const PriorityBadge = ({ priority }: { priority: string }) => (
  <span style={{background: colorFor(priority), color: 'white', padding: '4px 8px', borderRadius: 999, fontSize:12}}>{priority}</span>
);

export default PriorityBadge;
