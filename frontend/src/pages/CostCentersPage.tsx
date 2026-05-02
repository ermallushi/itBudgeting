import React, { useState, useEffect, useCallback } from 'react';
import { AgGridReact } from 'ag-grid-react';
import { ColDef } from 'ag-grid-community';
import 'ag-grid-community/styles/ag-grid.css';
import 'ag-grid-community/styles/ag-theme-alpine.css';
import * as api from '../api/costCentersApi';
import { CostCenter } from '../types';
import Modal from '../components/Modal';
import ConfirmDialog from '../components/ConfirmDialog';

const CostCentersPage: React.FC = () => {
  const [rows, setRows] = useState<CostCenter[]>([]);
  const [showModal, setShowModal] = useState(false);
  const [editItem, setEditItem] = useState<CostCenter | null>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);
  const [form, setForm] = useState({ name: '', code: '', description: '' });

  const load = useCallback(async () => {
    try { setRows(await api.getAll()); } catch { setRows([]); }
  }, []);
  useEffect(() => { load(); }, [load]);

  const openCreate = () => { setEditItem(null); setForm({ name: '', code: '', description: '' }); setShowModal(true); };
  const openEdit = (row: CostCenter) => { setEditItem(row); setForm({ name: row.name, code: row.code, description: row.description ?? '' }); setShowModal(true); };

  const handleSave = async () => {
    try {
      if (editItem) await api.update(editItem.id, form);
      else await api.create(form);
      setShowModal(false); load();
    } catch { alert('Error saving'); }
  };

  const handleDelete = async () => {
    if (deleteId == null) return;
    try { await api.remove(deleteId); setDeleteId(null); load(); } catch { alert('Error deleting'); }
  };

  const columns: ColDef<CostCenter>[] = [
    { field: 'code', headerName: 'Code', width: 120 },
    { field: 'name', headerName: 'Name', flex: 2 },
    { field: 'description', headerName: 'Description', flex: 3 },
    {
      headerName: 'Actions', width: 160,
      cellRenderer: (p: { data: CostCenter }) => (
        <div style={{ display: 'flex', gap: 4, alignItems: 'center', height: '100%' }}>
          <button onClick={() => openEdit(p.data)} style={actionBtn}>Edit</button>
          <button onClick={() => setDeleteId(p.data.id)} style={{ ...actionBtn, background: '#dc3545', color: '#fff' }}>Delete</button>
        </div>
      ),
    },
  ];

  return (
    <div style={{ padding: 24 }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16 }}>
        <h2 style={{ margin: 0 }}>Cost Centers</h2>
        <button onClick={openCreate} style={primaryBtn}>+ New Cost Center</button>
      </div>
      <div className="ag-theme-alpine" style={{ height: 500 }}>
        <AgGridReact rowData={rows} columnDefs={columns} defaultColDef={{ resizable: true, sortable: true, filter: true }} />
      </div>
      {showModal && (
        <Modal title={editItem ? 'Edit Cost Center' : 'New Cost Center'} onClose={() => setShowModal(false)}>
          {(['name', 'code', 'description'] as const).map(f => (
            <div key={f} style={formField}>
              <label style={{ textTransform: 'capitalize' }}>{f}</label>
              <input style={inp} value={form[f]} onChange={e => setForm(v => ({ ...v, [f]: e.target.value }))} />
            </div>
          ))}
          <div style={{ display: 'flex', gap: 8, justifyContent: 'flex-end', marginTop: 16 }}>
            <button onClick={() => setShowModal(false)}>Cancel</button>
            <button onClick={handleSave} style={primaryBtn}>Save</button>
          </div>
        </Modal>
      )}
      {deleteId != null && (
        <ConfirmDialog message="Delete this cost center?" onConfirm={handleDelete} onCancel={() => setDeleteId(null)} />
      )}
    </div>
  );
};

const primaryBtn: React.CSSProperties = { padding: '8px 16px', background: '#0d6efd', color: '#fff', border: 'none', borderRadius: 4, cursor: 'pointer' };
const actionBtn: React.CSSProperties = { padding: '4px 8px', cursor: 'pointer', fontSize: 12, borderRadius: 3 };
const formField: React.CSSProperties = { marginBottom: 12 };
const inp: React.CSSProperties = { width: '100%', padding: '6px 8px', border: '1px solid #ccc', borderRadius: 4, marginTop: 4, boxSizing: 'border-box' };

export default CostCentersPage;
