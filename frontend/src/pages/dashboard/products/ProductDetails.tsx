import { useState } from 'react';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { StockBadge } from '@/components/common/StockBadge';
import { LoadingSpinner } from '@/components/common/LoadingSpinner';
import { Edit, Package2, Calendar, Tag, DollarSign, Warehouse } from 'lucide-react';
import { Product, StockOperationType, UpdateStockDto } from '@/types';
import { productService } from '@/services/productService';
import { useApiMutation } from '@/hooks/useApi';
import { formatDateTime } from '@/lib/utils';
import { toast } from 'sonner';

interface ProductDetailsProps {
  product: Product;
  onEdit: () => void;
  onClose: () => void;
}

export function ProductDetails({ product, onEdit, onClose }: ProductDetailsProps) {
  const [showStockDialog, setShowStockDialog] = useState(false);
  const [stockQuantity, setStockQuantity] = useState(0);
  const [stockOperation, setStockOperation] = useState<StockOperationType>(StockOperationType.Add);

  // Mutation para atualizar estoque
  const updateStockMutation = useApiMutation(
    ({ id, data }: { id: string; data: UpdateStockDto }) =>
      productService.updateStock(id, data),
    {
      onSuccess: () => {
        toast.success('Estoque atualizado com sucesso!');
        setShowStockDialog(false);
        onClose(); // Fechar detalhes para forçar refresh
      },
    }
  );

  const handleUpdateStock = () => {
    if (stockQuantity <= 0) {
      toast.error('Quantidade deve ser maior que zero');
      return;
    }

    updateStockMutation.mutate({
      id: product.id,
      data: {
        quantity: stockQuantity,
        operation: stockOperation,
      },
    });
  };

  const getOperationLabel = (operation: StockOperationType) => {
    switch (operation) {
      case StockOperationType.Add:
        return 'Adicionar ao estoque';
      case StockOperationType.Remove:
        return 'Remover do estoque';
      case StockOperationType.Set:
        return 'Definir estoque';
      default:
        return 'Operação';
    }
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-start justify-between">
        <div className="flex items-start space-x-4">
          <div className="w-16 h-16 bg-gray-100 rounded-lg flex items-center justify-center">
            <Package2 className="w-8 h-8 text-gray-500" />
          </div>
          <div>
            <h2 className="text-2xl font-bold">{product.name}</h2>
            {product.sku && (
              <p className="text-sm text-gray-500">SKU: {product.sku}</p>
            )}
            <div className="flex items-center space-x-2 mt-2">
              <Badge variant={product.isActive ? 'default' : 'secondary'}>
                {product.isActive ? 'Ativo' : 'Inativo'}
              </Badge>
              <StockBadge stock={product.stock} />
            </div>
          </div>
        </div>
        <Button onClick={onEdit}>
          <Edit className="w-4 h-4 mr-2" />
          Editar
        </Button>
      </div>

      {/* Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Preço</CardTitle>
            <DollarSign className="h-4 w-4 text-green-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-green-600">
              {product.formattedPrice}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Estoque</CardTitle>
            <Warehouse className="h-4 w-4 text-blue-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{product.stock}</div>
            <p className="text-xs text-muted-foreground">{product.stockStatus}</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Categoria</CardTitle>
            <Tag className="h-4 w-4 text-purple-600" />
          </CardHeader>
          <CardContent>
            <div className="text-lg font-semibold">{product.categoryName}</div>
          </CardContent>
        </Card>
      </div>

      {/* Descrição */}
      {product.description && (
        <Card>
          <CardHeader>
            <CardTitle>Descrição</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-gray-700">{product.description}</p>
          </CardContent>
        </Card>
      )}

      {/* Informações */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center">
            <Calendar className="w-5 h-5 mr-2" />
            Informações
          </CardTitle>
        </CardHeader>
        <CardContent>
          <dl className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <dt className="text-sm font-medium text-gray-500">Criado em</dt>
              <dd className="text-sm text-gray-900">{formatDateTime(product.createdAt)}</dd>
            </div>
            <div>
              <dt className="text-sm font-medium text-gray-500">Última atualização</dt>
              <dd className="text-sm text-gray-900">{formatDateTime(product.updatedAt)}</dd>
            </div>
            {product.sku && (
              <div>
                <dt className="text-sm font-medium text-gray-500">SKU</dt>
                <dd className="text-sm text-gray-900">{product.sku}</dd>
              </div>
            )}
            <div>
              <dt className="text-sm font-medium text-gray-500">Moeda</dt>
              <dd className="text-sm text-gray-900">{product.currency}</dd>
            </div>
          </dl>
        </CardContent>
      </Card>

      {/* Actions */}
      <div className="flex justify-between pt-6 border-t">
        <Button variant="outline" onClick={() => setShowStockDialog(true)}>
          <Warehouse className="w-4 h-4 mr-2" />
          Atualizar Estoque
        </Button>
        <Button variant="outline" onClick={onClose}>
          Fechar
        </Button>
      </div>

      {/* Update Stock Dialog */}
      <Dialog open={showStockDialog} onOpenChange={setShowStockDialog}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Atualizar Estoque</DialogTitle>
          </DialogHeader>
          <div className="space-y-4">
            <div>
              <Label htmlFor="operation">Operação</Label>
              <Select
                value={stockOperation.toString()}
                onValueChange={(value) => setStockOperation(parseInt(value) as StockOperationType)}
              >
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value={StockOperationType.Add.toString()}>
                    Adicionar ao estoque atual ({product.stock})
                  </SelectItem>
                  <SelectItem value={StockOperationType.Remove.toString()}>
                    Remover do estoque atual ({product.stock})
                  </SelectItem>
                  <SelectItem value={StockOperationType.Set.toString()}>
                    Definir novo estoque
                  </SelectItem>
                </SelectContent>
              </Select>
            </div>

            <div>
              <Label htmlFor="quantity">Quantidade</Label>
              <Input
                id="quantity"
                type="number"
                min="1"
                value={stockQuantity}
                onChange={(e) => setStockQuantity(parseInt(e.target.value) || 0)}
                placeholder="Digite a quantidade"
              />
            </div>

            {stockOperation !== StockOperationType.Set && (
              <div className="text-sm text-gray-500">
                Estoque atual: {product.stock} unidades
                {stockOperation === StockOperationType.Add && stockQuantity > 0 && (
                  <div>Novo estoque: {product.stock + stockQuantity} unidades</div>
                )}
                {stockOperation === StockOperationType.Remove && stockQuantity > 0 && (
                  <div>Novo estoque: {Math.max(0, product.stock - stockQuantity)} unidades</div>
                )}
              </div>
            )}

            <div className="flex justify-end space-x-3 pt-4">
              <Button variant="outline" onClick={() => setShowStockDialog(false)}>
                Cancelar
              </Button>
              <Button
                onClick={handleUpdateStock}
                disabled={updateStockMutation.isPending || stockQuantity <= 0}
              >
                {updateStockMutation.isPending && (
                  <LoadingSpinner size="sm" className="mr-2" />
                )}
                {getOperationLabel(stockOperation)}
              </Button>
            </div>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
}