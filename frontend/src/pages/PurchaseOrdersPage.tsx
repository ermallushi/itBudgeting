import React, { useState, useEffect, useCallback } from 'react';
import { AgGridReact } from 'ag-grid-react';
import { ColDef, RowClassParams, RowStyle } from 'ag-grid-community';
import 'ag-grid-community/styles/ag-grid.css';
import 'ag-grid-community/styles/ag-theme-alpine.css';
import * as api from '../api/purchaseOrdersApi';
import { PurchaseOrder } from '../types';
import Modal from '../components/Modal';
import StatusBadge from '../components/StatusBadge';

const PurchaseOrdersPage: React.FC = () => {
  const [rows, setRows] = useState<PurchaseOrder[]>([]);
  const [selectedId, setSelectedId] = useState<number | null>(null);
  const [usageAmount, setUsageAmount] = useState(0);

  const load = useCallback(async () => {
    try { setRows(await api.getAll()); } catch { setRows([]); }
  }, []);
  useEffect(() => { load(); }, [load]);

  const handleRecordUsage = async () => {
    if (!selectedId) return;
    try { await api.recordUsage(selectedId, usageAmount); setSelectedId(null); load(); } catch { alert('Error recording usage'); }
  };

  const getRowStyle = (p: RowClassParams<PurchaseOrder>): RowStyle | undefined => {
    const r = p.data;
    if (r && r.amountApproved > 0 && r.remaining / r.amountApproved < 0.1) return { background: '#f8d7da' };
    return undefined;
  };

  const columns: ColDef<PurchaseOrder>[] = [
    { field: 'id', width: 70, headerName: 'PO#' },
    { field: 'purchaseRequestId', headerName: 'PR#', width: 80 },
    { field: 'amountApproved', headerName: 'Approved', width: 120, valueFormatter: p => `$${p.value?.toLocaleString()}` },
    { field: 'amountUsed', headerName: 'Used', width: 100, valueFormatter: p => `$${p.value?.toLocaleString()}` },
    { field: 'remaining', headerName: 'Remaining', width: 110, valueFormatter: p => `$${p.value?.toLocaleString()}` },
    { field: 'status', width: 130, cellRenderer: (p: { value: string }) => <StatusBadge status={p.value} /> },
    {
      headerName: 'Actions', width: 150,
      cellRenderer: (p: { data: PurchaseOrder }) => (
        <div style={{ display: 'flex', alignItems: 'center', height: '100%' }}>
          <button onClick={() => { setSelectedId(p.data.id); setUsageAmount(0); }} style={actionBtn}>Record Usage</button>
        </div>
      ),
    },
  ];

  return (
    <div style={{ padding: 24 }}>
      <h2 style={{ marginBottom: 16 }}>Purchase Orders</h2>
      <div className="ag-theme-alpine" style={{ height: 500 }}>
        <AgGridReact rowData={rows} columnDefs={columns} getRowStyle={getRowStyle} defaultColDef={{ resizable: true, sortable: true }} />
      </div>
      {selectedId && (
        <Modal title="Record Usage" onClose={() => setSelectedId(null)}>
          <div style={formField}>
            <label>Amount Used</label>
            <input style={inp} type="number" value={usageAmount} onChange={e => setUsageAmount(+e.target.value)} />
          </div>
          <div style={{ display: 'flex', gap: 8, justifyContent: 'flex-end', marginTop: 16 }}>
            <button onClick={() => setSelectedId(null)}>Cancel</button>
            <button onClick={handleRecordUsage} style={primaryBtn}>Save</button>
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

export default PurchaseOrdersPage;
