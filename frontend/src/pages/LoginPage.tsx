import React from 'react';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';

const LoginPage = () => {
  const { signin } = useAuth();
  const nav = useNavigate();
  const doDemo = async () => { await signin('demo@example.com','demo', true); nav('/'); };
  return (
    <div style={{maxWidth:420,margin:'3rem auto'}}>
      <h2>Sign In</h2>
      <div className="container">
        <label>Email</label>
        <input />
        <label>Password</label>
        <input type="password" />
        <div style={{display:'flex',gap:8,marginTop:8}}>
          <button onClick={doDemo}>Sign in (demo)</button>
        </div>
        <div style={{marginTop:12,color:'var(--muted)'}}>Demo: demo@example.com / demo</div>
      </div>
    </div>
  );
};

export default LoginPage;
