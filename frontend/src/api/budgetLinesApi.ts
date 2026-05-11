import api from './axiosInstance';
import { BudgetLine } from '../types';

export const getByVersion = (versionId: number) => api.get<BudgetLine[]>(`/budgetlines?versionId=${versionId}`).then(r => r.data);
export const create = (data: Partial<BudgetLine>) => api.post<BudgetLine>('/budgetlines', data).then(r => r.data);
export const update = (id: number, data: Partial<BudgetLine>) => api.put<BudgetLine>(`/budgetlines/${id}`, data).then(r => r.data);
export const remove = (id: number) => api.delete(`/budgetlines/${id}`);
export const batchUpdate = (lines: Partial<BudgetLine>[]) => api.put('/budgetlines/batch', lines).then(r => r.data);
