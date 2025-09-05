import { api } from '@/lib/api';
import { Dashboard } from '@/types';

export interface GetDashboardParams {
  lowStockThreshold?: number;
  recentProductsCount?: number;
}

export const dashboardService = {
  // Obter dados do dashboard
  async getDashboard(params?: GetDashboardParams): Promise<Dashboard> {
    const response = await api.get('/dashboard', { params });
    return response.data;
  },
};