import api from './axiosInstance';
import { Project } from '../types';

export const getAll = () => api.get<Project[]>('/projects').then(r => r.data);
export const getById = (id: number) => api.get<Project>(`/projects/${id}`).then(r => r.data);
export const create = (data: Partial<Project>) => api.post<Project>('/projects', data).then(r => r.data);
export const update = (id: number, data: Partial<Project>) => api.put<Project>(`/projects/${id}`, data).then(r => r.data);
export const remove = (id: number) => api.delete(`/projects/${id}`);
