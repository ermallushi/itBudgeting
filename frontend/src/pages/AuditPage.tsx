import React, { useState, useEffect, useCallback } from 'react';
import { AgGridReact } from 'ag-grid-react';
import { ColDef } from 'ag-grid-community';
import 'ag-grid-community/styles/ag-grid.css';
import 'ag-grid-community/styles/ag-theme-alpine.css';
import * as api from '../api/auditApi';
import { BudgetAudit } from '../types';

const ENTITY_TYPES = ['', 'BudgetVersion', 'BudgetLine', 'CostCenter', 'Project', 'PurchaseRequest', 'PurchaseOrder', 'BudgetTransfer'];

const AuditPage: React.FC = () => {
  const [rows, setRows] = useState<BudgetAudit[]>([]);
  const [filterType, setFilterType] = useState('');

  const load = useCallback(async () => {
    try { setRows(await api.getAll(filterType || undefined)); } catch { setRows([]); }
  }, [filterType]);
  useEffect(() => { load(); }, [load]);

  const columns: ColDef<BudgetAudit>[] = [
    { field: 'entityType', headerName: 'Entity Type', width: 140 },
    { field: 'entityId', headerName: 'Entity ID', width: 90 },
    { field: 'field', headerName: 'Field', width: 120 },
    { field: 'oldValue', headerName: 'Old Value', flex: 2 },
    { field: 'newValue', headerName: 'New Value', flex: 2 },
    { field: 'changedBy', headerName: 'Changed By', width: 120 },
    { field: 'changedAt', headerName: 'Changed At', width: 140, valueFormatter: p => p.value ? new Date(p.value).toLocaleString() : '' },
  ];

  return (
    <div style={{ padding: 24 }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16, alignItems: 'center' }}>
        <h2 style={{ margin: 0 }}>Audit Log</h2>
        <select style={{ padding: '6px 10px', border: '1px solid #ccc', borderRadius: 4 }}
          value={filterType} onChange={e => setFilterType(e.target.value)}>
          {ENTITY_TYPES.map(t => <option key={t} value={t}>{t || 'All Entity Types'}</option>)}
        </select>
      </div>
      <div className="ag-theme-alpine" style={{ height: 550 }}>
        <AgGridReact rowData={rows} columnDefs={columns} defaultColDef={{ resizable: true, sortable: true, filter: true }} />
      </div>
    </div>
  );
};

export default AuditPage;
