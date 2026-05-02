import React from 'react';
import { Role } from '../types';

const ROLE_COLORS: Record<Role, { bg: string; color: string }> = {
  Admin: { bg: '#d4edda', color: '#155724' },
  Manager: { bg: '#d1ecf1', color: '#0c5460' },
  FinanceController: { bg: '#fff3cd', color: '#856404' },
  BudgetUser: { bg: '#e2e3e5', color: '#383d41' },
};

const RoleBadge: React.FC<{ role: Role }> = ({ role }) => {
  const colors = ROLE_COLORS[role] || { bg: '#e2e3e5', color: '#383d41' };
  return (
    <span style={{
      background: colors.bg, color: colors.color,
      padding: '2px 10px', borderRadius: 12, fontSize: 12, fontWeight: 600,
    }}>
      {role}
    </span>
  );
};

export default RoleBadge;
