import React from 'react';

const StatCard = ({ title, value, caption }: { title: string; value: React.ReactNode; caption?: string }) => {
  return (
    <div className="container">
      <div style={{fontSize:12,color:'var(--muted)'}}>{title}</div>
      <div style={{fontSize:22,fontWeight:700,marginTop:6}}>{value}</div>
      {caption && <div style={{fontSize:12,color:'var(--muted)',marginTop:6}}>{caption}</div>}
    </div>
  );
};

export default StatCard;
