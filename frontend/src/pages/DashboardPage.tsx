import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import RoleBadge from '../components/RoleBadge';

const DashboardPage: React.FC = () => {
  const { user } = useAuth();

  const cards = [
    { label: 'Total Budget', value: '$1,200,000', color: '#d4edda' },
    { label: 'Committed', value: '$450,000', color: '#d1ecf1' },
    { label: 'Actual', value: '$380,000', color: '#fff3cd' },
    { label: 'Remaining', value: '$370,000', color: '#f8d7da' },
  ];

  const links = [
    { to: '/budget-versions', label: '📋 Budget Versions' },
    { to: '/projects', label: '📁 Projects' },
    { to: '/cost-centers', label: '🏢 Cost Centers' },
    { to: '/purchase-requests', label: '🛒 Purchase Requests' },
    { to: '/purchase-orders', label: '📦 Purchase Orders' },
    { to: '/budget-transfers', label: '🔄 Budget Transfers' },
    { to: '/reports', label: '📊 Reports' },
    { to: '/audit', label: '🔍 Audit Log' },
  ];

  return (
    <div style={{ padding: 24 }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 12, marginBottom: 24 }}>
        <h2 style={{ margin: 0 }}>Dashboard</h2>
        {user && <RoleBadge role={user.role} />}
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: 16, marginBottom: 32 }}>
        {cards.map(c => (
          <div key={c.label} style={{ background: c.color, padding: 20, borderRadius: 8, boxShadow: '0 1px 4px rgba(0,0,0,0.1)' }}>
            <div style={{ fontSize: 13, color: '#555' }}>{c.label}</div>
            <div style={{ fontSize: 24, fontWeight: 700, marginTop: 8 }}>{c.value}</div>
          </div>
        ))}
      </div>

      <h3>Quick Links</h3>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: 12 }}>
        {links.map(l => (
          <Link key={l.to} to={l.to} style={linkCard}>{l.label}</Link>
        ))}
      </div>
    </div>
  );
};

const linkCard: React.CSSProperties = {
  display: 'block', padding: 16, background: '#fff', border: '1px solid #e0e0e0',
  borderRadius: 8, textDecoration: 'none', color: '#333', fontWeight: 500,
  boxShadow: '0 1px 4px rgba(0,0,0,0.06)',
};

export default DashboardPage;
