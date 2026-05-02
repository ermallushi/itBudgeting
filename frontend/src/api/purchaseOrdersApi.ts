import api from './axiosInstance';
import { PurchaseOrder } from '../types';

export const getAll = () => api.get<PurchaseOrder[]>('/purchaseorders').then(r => r.data);
export const getById = (id: number) => api.get<PurchaseOrder>(`/purchaseorders/${id}`).then(r => r.data);
export const create = (data: Partial<PurchaseOrder>) => api.post<PurchaseOrder>('/purchaseorders', data).then(r => r.data);
export const update = (id: number, data: Partial<PurchaseOrder>) => api.put<PurchaseOrder>(`/purchaseorders/${id}`, data).then(r => r.data);
export const remove = (id: number) => api.delete(`/purchaseorders/${id}`);
export const recordUsage = (id: number, amount: number) => api.post(`/purchaseorders/${id}/recordusage`, { amount }).then(r => r.data);
