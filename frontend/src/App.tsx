import React from 'react';
import { BrowserRouter, Routes, Route, Navigate, Link, useNavigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import LoginPage from './pages/LoginPage';
import DashboardPage from './pages/DashboardPage';
import BudgetVersionsPage from './pages/BudgetVersionsPage';
import BudgetLinesPage from './pages/BudgetLinesPage';
import CostCentersPage from './pages/CostCentersPage';
import ProjectsPage from './pages/ProjectsPage';
import PurchaseRequestsPage from './pages/PurchaseRequestsPage';
import PurchaseOrdersPage from './pages/PurchaseOrdersPage';
import BudgetTransfersPage from './pages/BudgetTransfersPage';
import BudgetPeriodsPage from './pages/BudgetPeriodsPage';
import ReportsPage from './pages/ReportsPage';
import AuditPage from './pages/AuditPage';

const NAV_LINKS = [
  { to: '/', label: 'Dashboard' },
  { to: '/budget-versions', label: 'Budget Versions' },
  { to: '/cost-centers', label: 'Cost Centers' },
  { to: '/projects', label: 'Projects' },
  { to: '/purchase-requests', label: 'Purchase Requests' },
  { to: '/purchase-orders', label: 'Purchase Orders' },
  { to: '/budget-transfers', label: 'Budget Transfers' },
  { to: '/reports', label: 'Reports' },
  { to: '/audit', label: 'Audit Log' },
];

const AppLayout: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => { logout(); navigate('/login'); };

  return (
    <div style={{ display: 'flex', height: '100vh', fontFamily: 'sans-serif' }}>
      <nav style={navStyle}>
        <div style={{ padding: '16px 12px', borderBottom: '1px solid #333', fontWeight: 700, fontSize: 15 }}>
          💰 IT Budgeting
        </div>
        {NAV_LINKS.map(l => (
          <Link key={l.to} to={l.to} style={navLink}>{l.label}</Link>
        ))}
        <div style={{ marginTop: 'auto', padding: 12, borderTop: '1px solid #333' }}>
          <div style={{ color: '#aaa', fontSize: 12, marginBottom: 6 }}>{user?.username} ({user?.role})</div>
          <button onClick={handleLogout} style={logoutBtn}>Logout</button>
        </div>
      </nav>
      <main style={{ flex: 1, overflow: 'auto', background: '#f8f9fa' }}>
        {children}
      </main>
    </div>
  );
};

const ProtectedRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { user } = useAuth();
  if (!user) return <Navigate to="/login" replace />;
  return <>{children}</>;
};

const App: React.FC = () => (
  <AuthProvider>
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/*" element={
          <ProtectedRoute>
            <AppLayout>
              <Routes>
                <Route path="/" element={<DashboardPage />} />
                <Route path="/budget-versions" element={<BudgetVersionsPage />} />
                <Route path="/budget-versions/:id/lines" element={<BudgetLinesPage />} />
                <Route path="/cost-centers" element={<CostCentersPage />} />
                <Route path="/projects" element={<ProjectsPage />} />
                <Route path="/purchase-requests" element={<PurchaseRequestsPage />} />
                <Route path="/purchase-orders" element={<PurchaseOrdersPage />} />
                <Route path="/budget-transfers" element={<BudgetTransfersPage />} />
                <Route path="/budget-periods/:versionId" element={<BudgetPeriodsPage />} />
                <Route path="/reports" element={<ReportsPage />} />
                <Route path="/audit" element={<AuditPage />} />
              </Routes>
            </AppLayout>
          </ProtectedRoute>
        } />
      </Routes>
    </BrowserRouter>
  </AuthProvider>
);

const navStyle: React.CSSProperties = {
  width: 200, background: '#1a1a2e', color: '#fff', display: 'flex', flexDirection: 'column',
  flexShrink: 0, overflowY: 'auto',
};
const navLink: React.CSSProperties = {
  display: 'block', padding: '10px 16px', color: '#ccc', textDecoration: 'none', fontSize: 14,
};
const logoutBtn: React.CSSProperties = {
  width: '100%', padding: '6px', background: '#dc3545', color: '#fff', border: 'none', borderRadius: 4, cursor: 'pointer',
};

export default App;
