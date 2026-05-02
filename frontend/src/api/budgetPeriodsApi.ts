import api from './axiosInstance';
import { BudgetPeriod } from '../types';

export const getByVersion = (versionId: number) => api.get<BudgetPeriod[]>(`/budgetperiods?versionId=${versionId}`).then(r => r.data);
export const update = (id: number, data: Partial<BudgetPeriod>) => api.put<BudgetPeriod>(`/budgetperiods/${id}`, data).then(r => r.data);
