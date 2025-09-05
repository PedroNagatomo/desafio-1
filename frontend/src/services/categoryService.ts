import { api } from '@/lib/api';
import { Category, CreateCategoryDto, UpdateCategoryDto, PagedResult } from '@/types';

export interface GetCategoriesParams {
  page?: number;
  pageSize?: number;
  search?: string;
  isActive?: boolean;
}

export const categoryService = {
  // Obter categorias paginadas
  async getCategories(params?: GetCategoriesParams): Promise<PagedResult<Category>> {
    const response = await api.get('/categories', { params });
    return response.data;
  },

  // Obter categoria por ID
  async getCategory(id: string): Promise<Category> {
    const response = await api.get(`/categories/${id}`);
    return response.data;
  },

  // Criar categoria
  async createCategory(category: CreateCategoryDto): Promise<Category> {
    const response = await api.post('/categories', category);
    return response.data;
  },

  // Atualizar categoria
  async updateCategory(id: string, category: UpdateCategoryDto): Promise<Category> {
    const response = await api.put(`/categories/${id}`, category);
    return response.data;
  },

  // Deletar categoria
  async deleteCategory(id: string): Promise<void> {
    await api.delete(`/categories/${id}`);
  },

  // Obter categorias ativas (para dropdowns)
  async getActiveCategories(): Promise<Category[]> {
    const response = await api.get('/categories', {
      params: { pageSize: 100, isActive: true }
    });
    return response.data.data;
  },
};