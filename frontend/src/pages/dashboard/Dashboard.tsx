import { AppLayout } from '@/components/layout/AppLayout';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import {
  Package,
  FolderOpen,
  AlertTriangle,
  DollarSign,
  TrendingUp,
  Eye,
  Edit,
} from 'lucide-react';
import { StockBadge } from '@/components/common/StockBadge';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, PieChart, Pie, Cell } from 'recharts';

// Dados mockados
const dashboardData = {
  stats: {
    totalProducts: 156,
    totalCategories: 8,
    lowStockProducts: 12,
    totalStockValue: 125430.50,
    formattedTotalStockValue: 'R$ 125.430,50',
    activeProducts: 142,
    inactiveProducts: 14,
  },
  lowStockProducts: [
    { id: '1', name: 'iPhone 14', categoryName: 'Eletrônicos', stock: 3, formattedPrice: 'R$ 4.999,99' },
    { id: '2', name: 'Notebook Dell', categoryName: 'Eletrônicos', stock: 8, formattedPrice: 'R$ 2.499,99' },
    { id: '3', name: 'Cadeira Gamer', categoryName: 'Móveis', stock: 5, formattedPrice: 'R$ 899,99' },
    { id: '4', name: 'Mouse Gamer', categoryName: 'Eletrônicos', stock: 2, formattedPrice: 'R$ 159,99' },
    { id: '5', name: 'Teclado Mecânico', categoryName: 'Eletrônicos', stock: 7, formattedPrice: 'R$ 299,99' },
  ],
  categoryStats: [
    { categoryName: 'Eletrônicos', productCount: 45, totalValue: 67500 },
    { categoryName: 'Roupas', productCount: 38, totalValue: 23400 },
    { categoryName: 'Casa & Jardim', productCount: 28, totalValue: 18900 },
    { categoryName: 'Esportes', productCount: 22, totalValue: 12600 },
    { categoryName: 'Livros', productCount: 15, totalValue: 2850 },
    { categoryName: 'Beleza', productCount: 8, totalValue: 1680 },
  ],
  recentProducts: [
    { id: '1', name: 'MacBook Pro M3', categoryName: 'Eletrônicos', formattedPrice: 'R$ 12.999,99', createdAt: '2024-01-15' },
    { id: '2', name: 'Tênis Nike Air', categoryName: 'Esportes', formattedPrice: 'R$ 599,99', createdAt: '2024-01-14' },
    { id: '3', name: 'Perfume Chanel', categoryName: 'Beleza', formattedPrice: 'R$ 459,99', createdAt: '2024-01-13' },
    { id: '4', name: 'Livro Clean Code', categoryName: 'Livros', formattedPrice: 'R$ 89,99', createdAt: '2024-01-12' },
  ],
};

const COLORS = ['#3B82F6', '#10B981', '#F59E0B', '#EF4444', '#8B5CF6', '#06B6D4'];

export function Dashboard() {
  const formatDate = (date: string) => {
    return new Date(date).toLocaleDateString('pt-BR');
  };

  return (
    <AppLayout title="Dashboard" subtitle="Visão geral do sistema">
      <div className="space-y-6">
        {/* Stats Cards */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Total de Produtos</CardTitle>
              <Package className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{dashboardData.stats.totalProducts}</div>
              <p className="text-xs text-muted-foreground">
                {dashboardData.stats.activeProducts} ativos, {dashboardData.stats.inactiveProducts} inativos
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Categorias</CardTitle>
              <FolderOpen className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{dashboardData.stats.totalCategories}</div>
              <p className="text-xs text-muted-foreground">
                {dashboardData.categoryStats.length} com produtos
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Estoque Baixo</CardTitle>
              <AlertTriangle className="h-4 w-4 text-yellow-600" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-yellow-600">
                {dashboardData.stats.lowStockProducts}
              </div>
              <p className="text-xs text-muted-foreground">
                Produtos com menos de 10 unidades
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Valor Total</CardTitle>
              <DollarSign className="h-4 w-4 text-green-600" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-green-600">
                {dashboardData.stats.formattedTotalStockValue}
              </div>
              <p className="text-xs text-muted-foreground">
                Valor total em estoque
              </p>
            </CardContent>
          </Card>
        </div>

        {/* Charts */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <Card>
            <CardHeader>
              <CardTitle>Produtos por Categoria</CardTitle>
            </CardHeader>
            <CardContent>
              <ResponsiveContainer width="100%" height={300}>
                <BarChart data={dashboardData.categoryStats}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis 
                    dataKey="categoryName" 
                    tick={{ fontSize: 12 }}
                    angle={-45}
                    textAnchor="end"
                    height={80}
                  />
                  <YAxis />
                  <Tooltip />
                  <Bar dataKey="productCount" fill="#3B82F6" />
                </BarChart>
              </ResponsiveContainer>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Distribuição por Categoria</CardTitle>
            </CardHeader>
            <CardContent>
              <ResponsiveContainer width="100%" height={300}>
                <PieChart>
                  <Pie
                    data={dashboardData.categoryStats}
                    cx="50%"
                    cy="50%"
                    labelLine={false}
                    label={({ categoryName, productCount }) => `${categoryName}: ${productCount}`}
                    outerRadius={80}
                    fill="#8884d8"
                    dataKey="productCount"
                  >
                    {dashboardData.categoryStats.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                    ))}
                  </Pie>
                  <Tooltip />
                </PieChart>
              </ResponsiveContainer>
            </CardContent>
          </Card>
        </div>

        {/* Tables */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between">
              <CardTitle>Produtos com Estoque Baixo</CardTitle>
              <Badge variant="outline" className="text-yellow-600">
                {dashboardData.lowStockProducts.length} produtos
              </Badge>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                {dashboardData.lowStockProducts.map((product) => (
                  <div key={product.id} className="flex items-center justify-between p-3 border rounded-lg">
                    <div className="flex-1">
                      <h4 className="text-sm font-medium">{product.name}</h4>
                      <p className="text-xs text-gray-500">{product.categoryName}</p>
                    </div>
                    <div className="flex items-center space-x-2">
                      <StockBadge stock={product.stock} showQuantity />
                      <Button variant="ghost" size="sm">
                        <Eye className="w-4 h-4" />
                      </Button>
                    </div>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Produtos Recentes</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                {dashboardData.recentProducts.map((product) => (
                  <div key={product.id} className="flex items-center justify-between p-3 border rounded-lg">
                    <div className="flex-1">
                      <h4 className="text-sm font-medium">{product.name}</h4>
                      <p className="text-xs text-gray-500">
                        {product.categoryName} • {formatDate(product.createdAt)}
                      </p>
                    </div>
                    <div className="flex items-center space-x-2">
                      <span className="text-sm font-medium text-green-600">
                        {product.formattedPrice}
                      </span>
                      <Button variant="ghost" size="sm">
                        <Edit className="w-4 h-4" />
                      </Button>
                    </div>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </AppLayout>
  );
}