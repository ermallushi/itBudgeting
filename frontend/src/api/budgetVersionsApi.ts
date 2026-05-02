import api from './axiosInstance';
import { BudgetVersion } from '../types';

export const getAll = () => api.get<BudgetVersion[]>('/budgetversions').then(r => r.data);
export const getById = (id: number) => api.get<BudgetVersion>(`/budgetversions/${id}`).then(r => r.data);
export const create = (data: Partial<BudgetVersion>) => api.post<BudgetVersion>('/budgetversions', data).then(r => r.data);
export const update = (id: number, data: Partial<BudgetVersion>) => api.put<BudgetVersion>(`/budgetversions/${id}`, data).then(r => r.data);
export const remove = (id: number) => api.delete(`/budgetversions/${id}`);
export const submit = (id: number) => api.post(`/budgetversions/${id}/submit`).then(r => r.data);
export const approve = (id: number) => api.post(`/budgetversions/${id}/approve`).then(r => r.data);
export const lock = (id: number) => api.post(`/budgetversions/${id}/lock`).then(r => r.data);
export const cloneRevision = (id: number) => api.post(`/budgetversions/${id}/clone`).then(r => r.data);
