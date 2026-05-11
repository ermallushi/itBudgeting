import React, { useState, useEffect, useCallback } from 'react';
import { useParams } from 'react-router-dom';
import { AgGridReact } from 'ag-grid-react';
import { ColDef, CellClassParams } from 'ag-grid-community';
import 'ag-grid-community/styles/ag-grid.css';
import 'ag-grid-community/styles/ag-theme-alpine.css';
import * as linesApi from '../api/budgetLinesApi';
import * as versionsApi from '../api/budgetVersionsApi';
import { BudgetLine, BudgetVersion, BudgetVersionStatus } from '../types';

const MONTHS = ['jan','feb','mar','apr','may','jun','jul','aug','sep','oct','nov','dec'] as const;

const BudgetLinesPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const versionId = Number(id);
  const [rows, setRows] = useState<BudgetLine[]>([]);
  const [version, setVersion] = useState<BudgetVersion | null>(null);
  const [modified, setModified] = useState<Partial<BudgetLine>[]>([]);

  const load = useCallback(async () => {
    try {
      const [v, lines] = await Promise.all([
        versionsApi.getById(versionId),
        linesApi.getByVersion(versionId),
      ]);
      setVersion(v);
      setRows(lines);
    } catch {
      setRows([]);
    }
  }, [versionId]);

  useEffect(() => { load(); }, [load]);

  const isDraft = version?.status === BudgetVersionStatus.Draft;

  const getCellStyle = (params: CellClassParams<BudgetLine>) => {
    const committed = params.data?.committed ?? 0;
    const approved = params.data?.approved ?? 1;
    const ratio = committed / approved;
    if (ratio > 1) return { background: '#f8d7da' };
    if (ratio > 0.8) return { background: '#fff3cd' };
    return { background: '#d4edda' };
  };

  const monthCols: ColDef<BudgetLine>[] = MONTHS.map(m => ({
    field: m,
    headerName: m.charAt(0).toUpperCase() + m.slice(1),
    width: 80,
    editable: isDraft,
    cellStyle: isDraft ? undefined : { background: '#f5f5f5', color: '#aaa' },
  }));

  const columns: ColDef<BudgetLine>[] = [
    { field: 'costCenterName', headerName: 'Cost Center', width: 140 },
    { field: 'projectName', headerName: 'Project', width: 140 },
    { field: 'category', headerName: 'Category', width: 90 },
    ...monthCols,
    { field: 'planned', headerName: 'Planned', width: 100, cellStyle: getCellStyle },
    { field: 'approved', headerName: 'Approved', width: 100 },
    { field: 'committed', headerName: 'Committed', width: 110, cellStyle: getCellStyle },
    { field: 'actual', headerName: 'Actual', width: 90 },
  ];

  const handleSave = async () => {
    if (!modified.length) return;
    try {
      await linesApi.batchUpdate(modified);
      setModified([]);
      load();
      alert('Saved!');
    } catch { alert('Save failed'); }
  };

  return (
    <div style={{ padding: 24 }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16 }}>
        <h2 style={{ margin: 0 }}>Budget Lines — {version?.name ?? `Version ${versionId}`}</h2>
        {isDraft && <button onClick={handleSave} style={primaryBtn}>💾 Save Changes</button>}
      </div>
      <div className="ag-theme-alpine" style={{ height: 550 }}>
        <AgGridReact
          rowData={rows}
          columnDefs={columns}
          defaultColDef={{ resizable: true, sortable: true }}
          onCellValueChanged={e => {
            setModified(prev => {
              const existing = prev.find(r => r.id === e.data.id);
              if (existing) return prev.map(r => r.id === e.data.id ? e.data : r);
              return [...prev, e.data];
            });
          }}
        />
      </div>
    </div>
  );
};

const primaryBtn: React.CSSProperties = { padding: '8px 16px', background: '#0d6efd', color: '#fff', border: 'none', borderRadius: 4, cursor: 'pointer' };

export default BudgetLinesPage;
