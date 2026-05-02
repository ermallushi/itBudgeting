import React, { useState, useEffect, useCallback } from 'react';
import { AgGridReact } from 'ag-grid-react';
import { ColDef, CellClassParams } from 'ag-grid-community';
import 'ag-grid-community/styles/ag-grid.css';
import 'ag-grid-community/styles/ag-theme-alpine.css';
import * as reportsApi from '../api/reportsApi';
import * as versionsApi from '../api/budgetVersionsApi';
import { BudgetVersion, ReportSummaryRow, ReportVarianceRow } from '../types';

const ReportsPage: React.FC = () => {
  const [tab, setTab] = useState<'summary' | 'variance'>('summary');
  const [versions, setVersions] = useState<BudgetVersion[]>([]);
  const [selectedVersion, setSelectedVersion] = useState<number>(0);
  const [summaryRows, setSummaryRows] = useState<ReportSummaryRow[]>([]);
  const [varianceRows, setVarianceRows] = useState<ReportVarianceRow[]>([]);

  useEffect(() => {
    versionsApi.getAll().then(v => { setVersions(v); if (v.length) setSelectedVersion(v[0].id); }).catch(() => {});
  }, []);

  const loadData = useCallback(async () => {
    if (!selectedVersion) return;
    try {
      const [s, v] = await Promise.all([reportsApi.getSummary(selectedVersion), reportsApi.getVariance(selectedVersion)]);
      setSummaryRows(s); setVarianceRows(v);
    } catch { setSummaryRows([]); setVarianceRows([]); }
  }, [selectedVersion]);

  useEffect(() => { loadData(); }, [loadData]);

  const summaryCols: ColDef<ReportSummaryRow>[] = [
    { field: 'projectName', headerName: 'Project', flex: 2 },
    { field: 'month', width: 80 },
    { field: 'planned', width: 100, valueFormatter: p => `$${(p.value ?? 0).toLocaleString()}` },
    { field: 'approved', width: 100, valueFormatter: p => `$${(p.value ?? 0).toLocaleString()}` },
    { field: 'committed', width: 110, valueFormatter: p => `$${(p.value ?? 0).toLocaleString()}` },
    { field: 'actual', width: 90, valueFormatter: p => `$${(p.value ?? 0).toLocaleString()}` },
  ];

  const varianceCols: ColDef<ReportVarianceRow>[] = [
    { field: 'projectName', headerName: 'Project', flex: 2 },
    { field: 'month', width: 80 },
    { field: 'planned', width: 100, valueFormatter: p => `$${(p.value ?? 0).toLocaleString()}` },
    { field: 'actual', width: 90, valueFormatter: p => `$${(p.value ?? 0).toLocaleString()}` },
    {
      field: 'variance', width: 110,
      valueFormatter: p => `$${(p.value ?? 0).toLocaleString()}`,
      cellStyle: (p: CellClassParams<ReportVarianceRow>) => {
        const v = p.value as number;
        return { background: v < 0 ? '#f8d7da' : v > 0 ? '#d4edda' : '' };
      },
    },
  ];

  return (
    <div style={{ padding: 24 }}>
      <h2 style={{ marginBottom: 16 }}>Reports</h2>
      <div style={{ display: 'flex', gap: 12, marginBottom: 16, alignItems: 'center' }}>
        <select style={{ padding: '6px 10px', border: '1px solid #ccc', borderRadius: 4 }}
          value={selectedVersion} onChange={e => setSelectedVersion(+e.target.value)}>
          {versions.map(v => <option key={v.id} value={v.id}>{v.name} ({v.year})</option>)}
        </select>
        <div style={{ display: 'flex', border: '1px solid #ccc', borderRadius: 4, overflow: 'hidden' }}>
          {(['summary', 'variance'] as const).map(t => (
            <button key={t} onClick={() => setTab(t)}
              style={{ padding: '6px 16px', cursor: 'pointer', background: tab === t ? '#0d6efd' : '#fff', color: tab === t ? '#fff' : '#333', border: 'none', textTransform: 'capitalize' }}>
              {t}
            </button>
          ))}
        </div>
      </div>
      <div className="ag-theme-alpine" style={{ height: 500 }}>
        {tab === 'summary'
          ? <AgGridReact rowData={summaryRows} columnDefs={summaryCols} defaultColDef={{ resizable: true, sortable: true }} />
          : <AgGridReact rowData={varianceRows} columnDefs={varianceCols} defaultColDef={{ resizable: true, sortable: true }} />
        }
      </div>
    </div>
  );
};

export default ReportsPage;
