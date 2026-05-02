import api from './axiosInstance';
import { BudgetTransfer } from '../types';

export const getAll = () => api.get<BudgetTransfer[]>('/budgettransfers').then(r => r.data);
export const getById = (id: number) => api.get<BudgetTransfer>(`/budgettransfers/${id}`).then(r => r.data);
export const create = (data: Partial<BudgetTransfer>) => api.post<BudgetTransfer>('/budgettransfers', data).then(r => r.data);
export const remove = (id: number) => api.delete(`/budgettransfers/${id}`);
