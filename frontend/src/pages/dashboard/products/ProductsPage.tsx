import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { AppLayout } from '@/components/layout/AppLayout';
import { PageHeader } from '@/components/layout/PageHeader';
import { DataTable, Column } from '@/components/common/DataTable';
import { StockBadge } from '@/components/common/StockBadge';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from '@/components/ui/dropdown-menu';
import { Eye, Edit, Trash2, MoreHorizontal, Package2 } from 'lucide-react';
import { Product } from '@/types';
import { productService, GetProductsParams } from '@/services/productService';
import { formatCurrency, formatDate } from '@/lib/utils';
import { ProductForm } from './ProductForm';
import { ProductDetails } from './ProductDetails';
import { useApiMutation } from '@/hooks/useApi';
import { toast } from 'sonner';

export function ProductsPage() {
  const [params, setParams] = useState<GetProductsParams>({
    page: 1,
    pageSize: 20,
  });
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);
  const [showCreateDialog, setShowCreateDialog] = useState(false);
  const [showEditDialog, setShowEditDialog] = useState(false);
  const [showDetailsDialog, setShowDetailsDialog] = useState(false);

  // Query para produtos
  const { data: productsData, isLoading, refetch } = useQuery({
    queryKey: ['products', params],
    queryFn: () => productService.getProducts(params),
  });

  // Mutation para deletar produto
  const deleteProductMutation = useApiMutation(
    productService.deleteProduct,
    {
      onSuccess: () => {
        toast.success('Produto deletado com sucesso!');
        refetch();
      },
      invalidateQueries: [['products']],
    }
  );

  const handlePageChange = (page: number) => {
    setParams(prev => ({ ...prev, page }));
  };

  const handlePageSizeChange = (pageSize: number) => {
    setParams(prev => ({ ...prev, pageSize, page: 1 }));
  };

  const handleSearch = (search: string) => {
    setParams(prev => ({ ...prev, search: search || undefined, page: 1 }));
  };

  const handleDeleteProduct = (product: Product) => {
    if (confirm(`Tem certeza que deseja deletar o produto "${product.name}"?`)) {
      deleteProductMutation.mutate(product.id);
    }
  };

  const handleViewProduct = (product: Product) => {
    setSelectedProduct(product);
    setShowDetailsDialog(true);
  };

  const handleEditProduct = (product: Product) => {
    setSelectedProduct(product);
    setShowEditDialog(true);
  };

  const handleFormSuccess = () => {
    setShowCreateDialog(false);
    setShowEditDialog(false);
    setSelectedProduct(null);
    refetch();
  };

  const columns: Column<Product>[] = [
    {
      key: 'name',
      header: 'Produto',
      render: (product) => (
        <div className="flex items-center space-x-3">
          <div className="w-10 h-10 bg-gray-100 rounded-lg flex items-center justify-center">
            <Package2 className="w-5 h-5 text-gray-500" />
          </div>
          <div>
            <div className="font-medium">{product.name}</div>
            {product.sku && (
              <div className="text-sm text-gray-500">SKU: {product.sku}</div>
            )}
          </div>
        </div>
      ),
    },
    {
      key: 'categoryName',
      header: 'Categoria',
      render: (product) => (
        <Badge variant="secondary">{product.categoryName || 'N/A'}</Badge>
      ),
    },
    {
      key: 'price',
      header: 'Preço',
      render: (product) => (
        <span className="font-medium">{product.formattedPrice}</span>
      ),
    },
    {
      key: 'stock',
      header: 'Estoque',
      render: (product) => <StockBadge stock={product.stock} showQuantity />,
    },
    {
      key: 'isActive',
      header: 'Status',
      render: (product) => (
        <Badge variant={product.isActive ? 'default' : 'secondary'}>
          {product.isActive ? 'Ativo' : 'Inativo'}
        </Badge>
      ),
    },
    {
      key: 'createdAt',
      header: 'Criado em',
      render: (product) => (
        <span className="text-sm text-gray-500">
          {formatDate(product.createdAt)}
        </span>
      ),
    },
    {
      key: 'actions',
      header: 'Ações',
      render: (product) => (
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" size="sm">
              <MoreHorizontal className="w-4 h-4" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            <DropdownMenuItem onClick={() => handleViewProduct(product)}>
              <Eye className="w-4 h-4 mr-2" />
              Visualizar
            </DropdownMenuItem>
            <DropdownMenuItem onClick={() => handleEditProduct(product)}>
              <Edit className="w-4 h-4 mr-2" />
              Editar
            </DropdownMenuItem>
            <DropdownMenuItem
              onClick={() => handleDeleteProduct(product)}
              className="text-red-600"
            >
              <Trash2 className="w-4 h-4 mr-2" />
              Deletar
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      ),
    },
  ];

  return (
    <AppLayout>
      <div className="space-y-6">
        <PageHeader
          title="Produtos"
          subtitle="Gerencie os produtos da sua loja"
          showAddButton
          onAddClick={() => setShowCreateDialog(true)}
        />

        <DataTable
          data={productsData?.data || []}
          columns={columns}
          loading={isLoading}
          pagination={productsData}
          onPageChange={handlePageChange}
          onPageSizeChange={handlePageSizeChange}
          onSearch={handleSearch}
          searchPlaceholder="Buscar produtos..."
          emptyTitle="Nenhum produto encontrado"
          emptyDescription="Comece criando seu primeiro produto"
          emptyActionLabel="Criar Produto"
          onEmptyAction={() => setShowCreateDialog(true)}
        />

        {/* Create Product Dialog */}
        <Dialog open={showCreateDialog} onOpenChange={setShowCreateDialog}>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle>Criar Produto</DialogTitle>
            </DialogHeader>
            <ProductForm
              onSuccess={handleFormSuccess}
              onCancel={() => setShowCreateDialog(false)}
            />
          </DialogContent>
        </Dialog>

        {/* Edit Product Dialog */}
        <Dialog open={showEditDialog} onOpenChange={setShowEditDialog}>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle>Editar Produto</DialogTitle>
            </DialogHeader>
            {selectedProduct && (
              <ProductForm
                product={selectedProduct}
                onSuccess={handleFormSuccess}
                onCancel={() => setShowEditDialog(false)}
              />
            )}
          </DialogContent>
        </Dialog>

        {/* Product Details Dialog */}
        <Dialog open={showDetailsDialog} onOpenChange={setShowDetailsDialog}>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle>Detalhes do Produto</DialogTitle>
            </DialogHeader>
            {selectedProduct && (
              <ProductDetails
                product={selectedProduct}
                onEdit={() => {
                  setShowDetailsDialog(false);
                  setShowEditDialog(true);
                }}
                onClose={() => setShowDetailsDialog(false)}
              />
            )}
          </DialogContent>
        </Dialog>
      </div>
    </AppLayout>
  );
}