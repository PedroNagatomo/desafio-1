import { useQuery } from '@tanstack/react-query';
import { AppLayout } from '@/components/layout/AppLayout';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Skeleton } from '@/components/ui/skeleton';
import {
  Package,
  FolderOpen,
  AlertTriangle,
  DollarSign,
  TrendingUp,
  TrendingDown,
  Eye,
  Edit,
} from 'lucide-react';
import { dashboardService } from '@/services/dashboardService';
import { formatCurrency, formatDate } from '@/lib/utils';
import { StockBadge } from '@/components/common/StockBadge';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, PieChart, Pie, Cell } from 'recharts';

export function Dashboard() {
  const { data: dashboard, isLoading } = useQuery({
    queryKey: ['dashboard'],
    queryFn: () => dashboardService.getDashboard(),
  });

  if (isLoading) {
    return (
      <AppLayout title="Dashboard" subtitle="Visão geral do sistema">
        <div className="space-y-6">
          {/* Stats Cards Skeleton */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            {Array.from({ length: 4 }).map((_, i) => (
              <Card key={i}>
                <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                  <Skeleton className="h-4 w-20" />
                  <Skeleton className="h-4 w-4 rounded" />
                </CardHeader>
                <CardContent>
                  <Skeleton className="h-8 w-16 mb-1" />
                  <Skeleton className="h-3 w-24" />
                </CardContent>
              </Card>
            ))}
          </div>
          
          {/* Charts Skeleton */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <Card>
              <CardHeader>
                <Skeleton className="h-5 w-32" />
              </CardHeader>
              <CardContent>
                <Skeleton className="h-64 w-full" />
              </CardContent>
            </Card>
            <Card>
              <CardHeader>
                <Skeleton className="h-5 w-32" />
              </CardHeader>
              <CardContent>
                <Skeleton className="h-64 w-full" />
              </CardContent>
            </Card>
          </div>
        </div>
      </AppLayout>
    );
  }

  if (!dashboard) {
    return (
      <AppLayout title="Dashboard" subtitle="Visão geral do sistema">
        <div className="flex items-center justify-center h-64">
          <p className="text-gray-500">Erro ao carregar dados do dashboard</p>
        </div>
      </AppLayout>
    );
  }

  // Dados para os gráficos
  const categoryChartData = dashboard.categoryStats.map(stat => ({
    name: stat.categoryName,
    products: stat.productCount,
    value: stat.totalValue
  }));

  const COLORS = ['#3B82F6', '#10B981', '#F59E0B', '#EF4444', '#8B5CF6', '#06B6D4'];

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
              <div className="text-2xl font-bold">{dashboard.stats.totalProducts}</div>
              <p className="text-xs text-muted-foreground">
                {dashboard.stats.activeProducts} ativos, {dashboard.stats.inactiveProducts} inativos
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Categorias</CardTitle>
              <FolderOpen className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{dashboard.stats.totalCategories}</div>
              <p className="text-xs text-muted-foreground">
                {dashboard.categoryStats.length} com produtos
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
                {dashboard.stats.lowStockProducts}
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
                {dashboard.stats.formattedTotalStockValue}
              </div>
              <p className="text-xs text-muted-foreground">
                Valor total em estoque
              </p>
            </CardContent>
          </Card>
        </div>

        {/* Charts */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {/* Produtos por Categoria */}
          <Card>
            <CardHeader>
              <CardTitle>Produtos por Categoria</CardTitle>
            </CardHeader>
            <CardContent>
              <ResponsiveContainer width="100%" height={300}>
                <BarChart data={categoryChartData}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis 
                    dataKey="name" 
                    tick={{ fontSize: 12 }}
                    angle={-45}
                    textAnchor="end"
                    height={80}
                  />
                  <YAxis />
                  <Tooltip />
                  <Bar dataKey="products" fill="#3B82F6" />
                </BarChart>
              </ResponsiveContainer>
            </CardContent>
          </Card>

          {/* Distribuição por Categoria */}
          <Card>
            <CardHeader>
              <CardTitle>Distribuição por Categoria</CardTitle>
            </CardHeader>
            <CardContent>
              <ResponsiveContainer width="100%" height={300}>
                <PieChart>
                  <Pie
                    data={categoryChartData}
                    cx="50%"
                    cy="50%"
                    labelLine={false}
                    label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
                    outerRadius={80}
                    fill="#8884d8"
                    dataKey="products"
                  >
                    {categoryChartData.map((entry, index) => (
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
          {/* Produtos com Estoque Baixo */}
          <Card>
            <CardHeader className="flex flex-row items-center justify-between">
              <CardTitle>Produtos com Estoque Baixo</CardTitle>
              <Badge variant="outline" className="text-yellow-600">
                {dashboard.lowStockProducts.length} produtos
              </Badge>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                {dashboard.lowStockProducts.length === 0 ? (
                  <p className="text-sm text-gray-500 text-center py-4">
                    Nenhum produto com estoque baixo
                  </p>
                ) : (
                  dashboard.lowStockProducts.slice(0, 5).map((product) => (
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
                  ))
                )}
                {dashboard.lowStockProducts.length > 5 && (
                  <Button variant="outline" className="w-full">
                    Ver todos ({dashboard.lowStockProducts.length})
                  </Button>
                )}
              </div>
            </CardContent>
          </Card>

          {/* Produtos Recentes */}
          <Card>
            <CardHeader>
              <CardTitle>Produtos Recentes</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                {dashboard.recentProducts.length === 0 ? (
                  <p className="text-sm text-gray-500 text-center py-4">
                    Nenhum produto recente
                  </p>
                ) : (
                  dashboard.recentProducts.map((product) => (
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
                  ))
                )}
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </AppLayout>
  );
}