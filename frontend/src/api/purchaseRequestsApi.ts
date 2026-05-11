import api from './axiosInstance';
import { PurchaseRequest } from '../types';

export const getAll = () => api.get<PurchaseRequest[]>('/purchaserequests').then(r => r.data);
export const getById = (id: number) => api.get<PurchaseRequest>(`/purchaserequests/${id}`).then(r => r.data);
export const create = (data: Partial<PurchaseRequest>) => api.post<PurchaseRequest>('/purchaserequests', data).then(r => r.data);
export const update = (id: number, data: Partial<PurchaseRequest>) => api.put<PurchaseRequest>(`/purchaserequests/${id}`, data).then(r => r.data);
export const remove = (id: number) => api.delete(`/purchaserequests/${id}`);
export const approvePR = (id: number) => api.post(`/purchaserequests/${id}/approve`).then(r => r.data);
