import React, { useState, useEffect, useCallback } from 'react';
import { AgGridReact } from 'ag-grid-react';
import { ColDef } from 'ag-grid-community';
import 'ag-grid-community/styles/ag-grid.css';
import 'ag-grid-community/styles/ag-theme-alpine.css';
import * as api from '../api/budgetTransfersApi';
import { BudgetTransfer, BudgetCategory } from '../types';
import Modal from '../components/Modal';
import { useAuth } from '../context/AuthContext';

const BudgetTransfersPage: React.FC = () => {
  const [rows, setRows] = useState<BudgetTransfer[]>([]);
  const [showModal, setShowModal] = useState(false);
  const [form, setForm] = useState({ fromProjectId: 0, toProjectId: 0, amount: 0, category: BudgetCategory.OpEx, period: '', reason: '' });
  const { user } = useAuth();

  const load = useCallback(async () => {
    try { setRows(await api.getAll()); } catch { setRows([]); }
  }, []);
  useEffect(() => { load(); }, [load]);

  const handleCreate = async () => {
    try {
      await api.create({ ...form, createdBy: user?.username ?? 'unknown' });
      setShowModal(false); load();
    } catch { alert('Error creating transfer'); }
  };

  const columns: ColDef<BudgetTransfer>[] = [
    { field: 'id', width: 70 },
    { field: 'fromProjectName', headerName: 'From Project', flex: 2 },
    { field: 'toProjectName', headerName: 'To Project', flex: 2 },
    { field: 'amount', width: 120, valueFormatter: p => `$${p.value?.toLocaleString()}` },
    { field: 'category', width: 90 },
    { field: 'period', width: 90 },
    { field: 'reason', flex: 3 },
    { field: 'createdBy', headerName: 'By', width: 100 },
    { field: 'createdAt', headerName: 'Date', width: 110, valueFormatter: p => p.value ? new Date(p.value).toLocaleDateString() : '' },
  ];

  return (
    <div style={{ padding: 24 }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16 }}>
        <h2 style={{ margin: 0 }}>Budget Transfers</h2>
        <button onClick={() => setShowModal(true)} style={primaryBtn}>+ Create Transfer</button>
      </div>
      <div className="ag-theme-alpine" style={{ height: 500 }}>
        <AgGridReact rowData={rows} columnDefs={columns} defaultColDef={{ resizable: true, sortable: true }} />
      </div>
      {showModal && (
        <Modal title="Create Budget Transfer" onClose={() => setShowModal(false)}>
          <div style={formField}><label>From Project ID</label><input style={inp} type="number" value={form.fromProjectId} onChange={e => setForm(v => ({ ...v, fromProjectId: +e.target.value }))} /></div>
          <div style={formField}><label>To Project ID</label><input style={inp} type="number" value={form.toProjectId} onChange={e => setForm(v => ({ ...v, toProjectId: +e.target.value }))} /></div>
          <div style={formField}><label>Amount</label><input style={inp} type="number" value={form.amount} onChange={e => setForm(v => ({ ...v, amount: +e.target.value }))} /></div>
          <div style={formField}>
            <label>Category</label>
            <select style={inp} value={form.category} onChange={e => setForm(v => ({ ...v, category: e.target.value as BudgetCategory }))}>
              {Object.values(BudgetCategory).map(c => <option key={c}>{c}</option>)}
            </select>
          </div>
          <div style={formField}><label>Period (e.g. 2024-01)</label><input style={inp} value={form.period} onChange={e => setForm(v => ({ ...v, period: e.target.value }))} /></div>
          <div style={formField}><label>Reason</label><textarea style={{ ...inp, minHeight: 60 }} value={form.reason} onChange={e => setForm(v => ({ ...v, reason: e.target.value }))} /></div>
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
const formField: React.CSSProperties = { marginBottom: 12 };
const inp: React.CSSProperties = { width: '100%', padding: '6px 8px', border: '1px solid #ccc', borderRadius: 4, marginTop: 4, boxSizing: 'border-box' };

export default BudgetTransfersPage;
