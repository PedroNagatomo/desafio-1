import { api } from '@/lib/api';
import { Product, CreateProductDto, UpdateProductDto, UpdateStockDto, PagedResult } from '@/types';

export interface GetProductsParams {
  page?: number;
  pageSize?: number;
  search?: string;
  categoryId?: string;
  isActive?: boolean;
  orderBy?: string;
  ascending?: boolean;
}

export const productService = {
  // Obter produtos paginados
  async getProducts(params?: GetProductsParams): Promise<PagedResult<Product>> {
    const response = await api.get('/products', { params });
    return response.data;
  },

  // Obter produto por ID
  async getProduct(id: string): Promise<Product> {
    const response = await api.get(`/products/${id}`);
    return response.data;
  },

  // Criar produto
  async createProduct(product: CreateProductDto): Promise<Product> {
    const response = await api.post('/products', product);
    return response.data;
  },

  // Atualizar produto
  async updateProduct(id: string, product: UpdateProductDto): Promise<Product> {
    const response = await api.put(`/products/${id}`, product);
    return response.data;
  },

  // Deletar produto
  async deleteProduct(id: string): Promise<void> {
    await api.delete(`/products/${id}`);
  },

  // Atualizar estoque
  async updateStock(id: string, stock: UpdateStockDto): Promise<Product> {
    const response = await api.patch(`/products/${id}/stock`, stock);
    return response.data;
  },

  // Obter produtos com estoque baixo
  async getLowStockProducts(threshold: number = 10): Promise<Product[]> {
    const response = await api.get('/products/low-stock', {
      params: { threshold }
    });
    return response.data;
  },
};