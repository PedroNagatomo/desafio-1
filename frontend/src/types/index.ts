export interface Product {
  id: string;
  name: string;
  description?: string;
  price: number;
  currency: string;
  categoryId: string;
  categoryName?: string;
  stock: number;
  isActive: boolean;
  sku?: string;
  isLowStock: boolean;
  createdAt: string;
  updatedAt: string;
  formattedPrice: string;
  stockStatus: string;
}

export interface Category {
  id: string;
  name: string;
  description?: string;
  isActive: boolean;
  productCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateProductDto {
  name: string;
  description?: string;
  price: number;
  categoryId: string;
  stock: number;
  sku?: string;
}

export interface UpdateProductDto {
  name: string;
  description?: string;
  price: number;
  categoryId: string;
  sku?: string;
}

export interface CreateCategoryDto {
  name: string;
  description?: string;
}

export interface UpdateCategoryDto {
  name: string;
  description?: string;
}

export interface UpdateStockDto {
  quantity: number;
  operation: StockOperationType;
}

export enum StockOperationType {
  Set = 0,
  Add = 1,
  Remove = 2,
}

export interface PagedResult<T> {
  data: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

export interface DashboardStats {
  totalProducts: number;
  totalCategories: number;
  lowStockProducts: number;
  totalStockValue: number;
  formattedTotalStockValue: string;
  activeProducts: number;
  inactiveProducts: number;
}

export interface CategoryStats {
  categoryId: string;
  categoryName: string;
  productCount: number;
  totalValue: number;
  formattedTotalValue: string;
}

export interface Dashboard {
  stats: DashboardStats;
  lowStockProducts: Product[];
  categoryStats: CategoryStats[];
  recentProducts: Product[];
}

export interface ApiError {
  error: string;
  details?: Array<{
    propertyName: string;
    errorMessage: string;
  }>;
  statusCode: number;
}