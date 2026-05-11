import api from './axiosInstance';
import { CostCenter } from '../types';

export const getAll = () => api.get<CostCenter[]>('/costcenters').then(r => r.data);
export const getById = (id: number) => api.get<CostCenter>(`/costcenters/${id}`).then(r => r.data);
export const create = (data: Partial<CostCenter>) => api.post<CostCenter>('/costcenters', data).then(r => r.data);
export const update = (id: number, data: Partial<CostCenter>) => api.put<CostCenter>(`/costcenters/${id}`, data).then(r => r.data);
export const remove = (id: number) => api.delete(`/costcenters/${id}`);
