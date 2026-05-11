import React, { useState, useEffect, useCallback } from 'react';
import { useParams } from 'react-router-dom';
import { AgGridReact } from 'ag-grid-react';
import { ColDef } from 'ag-grid-community';
import 'ag-grid-community/styles/ag-grid.css';
import 'ag-grid-community/styles/ag-theme-alpine.css';
import * as api from '../api/budgetPeriodsApi';
import { BudgetPeriod, PeriodLockType } from '../types';
import { useAuth } from '../context/AuthContext';

const BudgetPeriodsPage: React.FC = () => {
  const { versionId } = useParams<{ versionId: string }>();
  const id = Number(versionId);
  const [rows, setRows] = useState<BudgetPeriod[]>([]);
  const { hasRole } = useAuth();
  const canEdit = hasRole('Manager', 'Admin');

  const load = useCallback(async () => {
    try { setRows(await api.getByVersion(id)); } catch { setRows([]); }
  }, [id]);
  useEffect(() => { load(); }, [load]);

  const handleToggle = async (row: BudgetPeriod) => {
    if (!canEdit) return;
    try { await api.update(row.id, { isOpen: !row.isOpen }); load(); } catch { alert('Error'); }
  };

  const handleLockChange = async (row: BudgetPeriod, lockType: PeriodLockType) => {
    if (!canEdit) return;
    try { await api.update(row.id, { lockType }); load(); } catch { alert('Error'); }
  };

  const columns: ColDef<BudgetPeriod>[] = [
    { field: 'periodNumber', headerName: '#', width: 60 },
    { field: 'periodName', headerName: 'Period', width: 100 },
    {
      field: 'isOpen', headerName: 'Is Open', width: 100,
      cellRenderer: (p: { data: BudgetPeriod }) => (
        <input type="checkbox" checked={p.data.isOpen} disabled={!canEdit}
          onChange={() => handleToggle(p.data)} />
      ),
    },
    {
      field: 'lockType', headerName: 'Lock Type', width: 160,
      cellRenderer: (p: { data: BudgetPeriod }) => (
        <select disabled={!canEdit} value={p.data.lockType}
          onChange={e => handleLockChange(p.data, e.target.value as PeriodLockType)}>
          {Object.values(PeriodLockType).map(t => <option key={t}>{t}</option>)}
        </select>
      ),
    },
  ];

  return (
    <div style={{ padding: 24 }}>
      <h2 style={{ marginBottom: 16 }}>Budget Periods — Version {versionId}</h2>
      <div className="ag-theme-alpine" style={{ height: 500 }}>
        <AgGridReact rowData={rows} columnDefs={columns} defaultColDef={{ resizable: true }} />
      </div>
    </div>
  );
};

export default BudgetPeriodsPage;
