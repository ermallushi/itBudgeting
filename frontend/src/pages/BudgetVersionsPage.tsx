import React, { useState, useEffect, useCallback } from 'react';
import { AgGridReact } from 'ag-grid-react';
import { ColDef, RowClassParams, RowStyle } from 'ag-grid-community';
import 'ag-grid-community/styles/ag-grid.css';
import 'ag-grid-community/styles/ag-theme-alpine.css';
import { useNavigate } from 'react-router-dom';
import * as api from '../api/budgetVersionsApi';
import { BudgetVersion, BudgetVersionStatus, BudgetVersionType } from '../types';
import Modal from '../components/Modal';
import { useAuth } from '../context/AuthContext';
import StatusBadge from '../components/StatusBadge';

const BudgetVersionsPage: React.FC = () => {
  const [rows, setRows] = useState<BudgetVersion[]>([]);
  const [showModal, setShowModal] = useState(false);
  const [form, setForm] = useState({ name: '', year: new Date().getFullYear(), type: BudgetVersionType.Annual });
  const { hasRole } = useAuth();
  const navigate = useNavigate();

  const load = useCallback(async () => {
    try {
      const data = await api.getAll();
      setRows(data);
    } catch {
      setRows([]);
    }
  }, []);

  useEffect(() => { load(); }, [load]);

  const handleCreate = async () => {
    try {
      await api.create({ ...form, status: BudgetVersionStatus.Draft });
      setShowModal(false);
      load();
    } catch { alert('Error creating version'); }
  };

  const handleAction = async (action: string, id: number) => {
    try {
      if (action === 'submit') await api.submit(id);
      else if (action === 'approve') await api.approve(id);
      else if (action === 'lock') await api.lock(id);
      else if (action === 'clone') await api.cloneRevision(id);
      load();
    } catch { alert('Action failed'); }
  };

  const getRowStyle = (params: RowClassParams<BudgetVersion>): RowStyle | undefined => {
    const status = params.data?.status;
    if (status === BudgetVersionStatus.Approved) return { background: '#d4edda' };
    if (status === BudgetVersionStatus.Draft) return { background: '#fff3cd' };
    if (status === BudgetVersionStatus.Submitted) return { background: '#d1ecf1' };
    if (status === BudgetVersionStatus.Locked) return { background: '#f8d7da' };
    return undefined;
  };

  const columns: ColDef<BudgetVersion>[] = [
    { field: 'name', headerName: 'Name', flex: 2 },
    { field: 'year', headerName: 'Year', width: 80 },
    { field: 'type', headerName: 'Type', width: 100 },
    { field: 'status', headerName: 'Status', width: 120,
      cellRenderer: (p: { value: string }) => <StatusBadge status={p.value} /> },
    { field: 'createdBy', headerName: 'Created By', flex: 1 },
    { field: 'createdAt', headerName: 'Created At', flex: 1,
      valueFormatter: p => p.value ? new Date(p.value).toLocaleDateString() : '' },
    {
      headerName: 'Actions', flex: 2,
      cellRenderer: (p: { data: BudgetVersion }) => (
        <div style={{ display: 'flex', gap: 4, alignItems: 'center', height: '100%' }}>
          <button onClick={() => navigate(`/budget-versions/${p.data.id}/lines`)} style={actionBtn}>Lines</button>
          {p.data.status === BudgetVersionStatus.Draft && (
            <button onClick={() => handleAction('submit', p.data.id)} style={actionBtn}>Submit</button>
          )}
          {p.data.status === BudgetVersionStatus.Submitted && hasRole('Admin', 'FinanceController') && (
            <button onClick={() => handleAction('approve', p.data.id)} style={{ ...actionBtn, background: '#28a745', color: '#fff' }}>Approve</button>
          )}
          {p.data.status === BudgetVersionStatus.Approved && hasRole('Admin', 'FinanceController') && (
            <button onClick={() => handleAction('lock', p.data.id)} style={{ ...actionBtn, background: '#dc3545', color: '#fff' }}>Lock</button>
          )}
          <button onClick={() => handleAction('clone', p.data.id)} style={actionBtn}>Clone</button>
        </div>
      ),
    },
  ];

  return (
    <div style={{ padding: 24 }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16 }}>
        <h2 style={{ margin: 0 }}>Budget Versions</h2>
        <button onClick={() => setShowModal(true)} style={primaryBtn}>+ Create Version</button>
      </div>
      <div className="ag-theme-alpine" style={{ height: 500 }}>
        <AgGridReact rowData={rows} columnDefs={columns} getRowStyle={getRowStyle} />
      </div>
      {showModal && (
        <Modal title="Create Budget Version" onClose={() => setShowModal(false)}>
          <div style={formField}>
            <label>Name</label>
            <input style={inp} value={form.name} onChange={e => setForm(f => ({ ...f, name: e.target.value }))} />
          </div>
          <div style={formField}>
            <label>Year</label>
            <input style={inp} type="number" value={form.year} onChange={e => setForm(f => ({ ...f, year: +e.target.value }))} />
          </div>
          <div style={formField}>
            <label>Type</label>
            <select style={inp} value={form.type} onChange={e => setForm(f => ({ ...f, type: e.target.value as BudgetVersionType }))}>
              {Object.values(BudgetVersionType).map(t => <option key={t}>{t}</option>)}
            </select>
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

export default BudgetVersionsPage;
