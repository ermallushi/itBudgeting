import api from './axiosInstance';
import { ReportSummaryRow, ReportVarianceRow } from '../types';

export const getSummary = (versionId: number) => api.get<ReportSummaryRow[]>(`/reports/summary?versionId=${versionId}`).then(r => r.data);
export const getVariance = (versionId: number) => api.get<ReportVarianceRow[]>(`/reports/variance?versionId=${versionId}`).then(r => r.data);
