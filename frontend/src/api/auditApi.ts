import api from './axiosInstance';
import { BudgetAudit } from '../types';

export const getAll = (entityType?: string) => {
  const params = entityType ? `?entityType=${entityType}` : '';
  return api.get<BudgetAudit[]>(`/auditlog${params}`).then(r => r.data);
};
