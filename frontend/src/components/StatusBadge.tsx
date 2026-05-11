import React from 'react';

interface StatusBadgeProps {
  status: string;
}

const STATUS_COLORS: Record<string, { bg: string; color: string }> = {
  Draft: { bg: '#fff3cd', color: '#856404' },
  Submitted: { bg: '#d1ecf1', color: '#0c5460' },
  Approved: { bg: '#d4edda', color: '#155724' },
  Locked: { bg: '#f8d7da', color: '#721c24' },
  Rejected: { bg: '#f8d7da', color: '#721c24' },
  Open: { bg: '#d4edda', color: '#155724' },
  PartiallyUsed: { bg: '#fff3cd', color: '#856404' },
  FullyUsed: { bg: '#d1ecf1', color: '#0c5460' },
  Closed: { bg: '#e2e3e5', color: '#383d41' },
};

const StatusBadge: React.FC<StatusBadgeProps> = ({ status }) => {
  const colors = STATUS_COLORS[status] || { bg: '#e2e3e5', color: '#383d41' };
  return (
    <span style={{
      background: colors.bg, color: colors.color,
      padding: '2px 10px', borderRadius: 12, fontSize: 12, fontWeight: 600,
    }}>
      {status}
    </span>
  );
};

export default StatusBadge;
