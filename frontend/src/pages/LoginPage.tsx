import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const LoginPage: React.FC = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const ok = login(username, password);
    if (ok) {
      navigate('/');
    } else {
      setError('Invalid credentials. Use: admin, manager, financecontroller, or budgetuser');
    }
  };

  return (
    <div style={container}>
      <div style={card}>
        <h2 style={{ textAlign: 'center', marginBottom: 24 }}>IT Budgeting System</h2>
        <form onSubmit={handleSubmit}>
          <div style={field}>
            <label>Username</label>
            <input style={input} value={username} onChange={e => setUsername(e.target.value)} placeholder="admin / manager / financecontroller / budgetuser" />
          </div>
          <div style={field}>
            <label>Password</label>
            <input style={input} type="password" value={password} onChange={e => setPassword(e.target.value)} placeholder="Any password" />
          </div>
          {error && <p style={{ color: 'red', fontSize: 13 }}>{error}</p>}
          <button style={btn} type="submit">Login</button>
        </form>
      </div>
    </div>
  );
};

const container: React.CSSProperties = { display: 'flex', alignItems: 'center', justifyContent: 'center', height: '100vh', background: '#f0f2f5' };
const card: React.CSSProperties = { background: '#fff', padding: 32, borderRadius: 8, boxShadow: '0 2px 16px rgba(0,0,0,0.1)', width: 400 };
const field: React.CSSProperties = { marginBottom: 16 };
const input: React.CSSProperties = { width: '100%', padding: '8px 10px', border: '1px solid #ccc', borderRadius: 4, marginTop: 4, boxSizing: 'border-box' };
const btn: React.CSSProperties = { width: '100%', padding: '10px', background: '#0d6efd', color: '#fff', border: 'none', borderRadius: 4, cursor: 'pointer', fontSize: 15 };

export default LoginPage;
