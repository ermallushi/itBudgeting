import React, { useState, useEffect, useCallback } from 'react';
import { AgGridReact } from 'ag-grid-react';
import { ColDef, RowClassParams, RowStyle } from 'ag-grid-community';
import 'ag-grid-community/styles/ag-grid.css';
import 'ag-grid-community/styles/ag-theme-alpine.css';
import * as api from '../api/purchaseRequestsApi';
import { PurchaseRequest, PurchaseRequestStatus } from '../types';
import Modal from '../components/Modal';
import { useAuth } from '../context/AuthContext';
import StatusBadge from '../components/StatusBadge';

const PurchaseRequestsPage: React.FC = () => {
  const [rows, setRows] = useState<PurchaseRequest[]>([]);
  const [showModal, setShowModal] = useState(false);
  const [form, setForm] = useState({ projectId: 0, amount: 0, sharePointUrl: '' });
  const { hasRole, user } = useAuth();

  const load = useCallback(async () => {
    try { setRows(await api.getAll()); } catch { setRows([]); }
  }, []);
  useEffect(() => { load(); }, [load]);

  const handleCreate = async () => {
    try {
      await api.create({ ...form, status: PurchaseRequestStatus.Draft, createdBy: user?.username ?? 'unknown' });
      setShowModal(false); load();
    } catch { alert('Error creating PR'); }
  };

  const handleApprove = async (id: number) => {
    try { await api.approvePR(id); load(); } catch { alert('Error approving'); }
  };

  const getRowStyle = (p: RowClassParams<PurchaseRequest>): RowStyle | undefined => {
    const s = p.data?.status;
    if (s === PurchaseRequestStatus.Approved) return { background: '#d4edda' };
    if (s === PurchaseRequestStatus.Draft) return { background: '#fff3cd' };
    if (s === PurchaseRequestStatus.Submitted) return { background: '#d1ecf1' };
    if (s === PurchaseRequestStatus.Rejected) return { background: '#f8d7da' };
    return undefined;
  };

  const columns: ColDef<PurchaseRequest>[] = [
    { field: 'id', width: 70 },
    { field: 'projectName', headerName: 'Project', flex: 2 },
    { field: 'amount', headerName: 'Amount', width: 120, valueFormatter: p => `$${p.value?.toLocaleString()}` },
    { field: 'status', width: 120, cellRenderer: (p: { value: string }) => <StatusBadge status={p.value} /> },
    { field: 'sharePointUrl', headerName: 'SharePoint URL', flex: 2 },
    { field: 'createdBy', headerName: 'Created By', width: 120 },
    { field: 'createdAt', headerName: 'Created', width: 120, valueFormatter: p => p.value ? new Date(p.value).toLocaleDateString() : '' },
    {
      headerName: 'Actions', width: 130,
      cellRenderer: (p: { data: PurchaseRequest }) => (
        hasRole('Manager', 'Admin') && p.data.status === PurchaseRequestStatus.Submitted ? (
          <div style={{ display: 'flex', alignItems: 'center', height: '100%' }}>
            <button onClick={() => handleApprove(p.data.id)} style={{ ...actionBtn, background: '#28a745', color: '#fff' }}>Approve</button>
          </div>
        ) : null
      ),
    },
  ];

  return (
    <div style={{ padding: 24 }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16 }}>
        <h2 style={{ margin: 0 }}>Purchase Requests</h2>
        <button onClick={() => setShowModal(true)} style={primaryBtn}>+ Create PR</button>
      </div>
      <div className="ag-theme-alpine" style={{ height: 500 }}>
        <AgGridReact rowData={rows} columnDefs={columns} getRowStyle={getRowStyle} defaultColDef={{ resizable: true, sortable: true }} />
      </div>
      {showModal && (
        <Modal title="Create Purchase Request" onClose={() => setShowModal(false)}>
          <div style={formField}>
            <label>Project ID</label>
            <input style={inp} type="number" value={form.projectId} onChange={e => setForm(v => ({ ...v, projectId: +e.target.value }))} />
          </div>
          <div style={formField}>
            <label>Amount</label>
            <input style={inp} type="number" value={form.amount} onChange={e => setForm(v => ({ ...v, amount: +e.target.value }))} />
          </div>
          <div style={formField}>
            <label>SharePoint URL</label>
            <input style={inp} value={form.sharePointUrl} onChange={e => setForm(v => ({ ...v, sharePointUrl: e.target.value }))} />
          </div>
          <div style={{ display: 'flex', gap: 8, justifyContent: 'flex-end', marginTop: 16 }}>
            <button onClick={() => setShowModal(false)}>Cancel</button>
            <button onClick={handleCreate} style={primaryBtn}>Create</button>
          </div>
        </Modal>
      )}
    </div>
  );
};

const primaryBtn: React.CSSProperties = { padding: '8px 16px', background: '#0d6efd', color: '#fff', border: 'none', borderRadius: 4, cursor: 'pointer' };
const actionBtn: React.CSSProperties = { padding: '4px 8px', cursor: 'pointer', fontSize: 12, borderRadius: 3 };
const formField: React.CSSProperties = { marginBottom: 12 };
const inp: React.CSSProperties = { width: '100%', padding: '6px 8px', border: '1px solid #ccc', borderRadius: 4, marginTop: 4, boxSizing: 'border-box' };

export default PurchaseRequestsPage;
