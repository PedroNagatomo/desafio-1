import { Badge } from '@/components/ui/badge';
import { getStockStatusBadgeVariant } from '@/lib/utils';

interface StockBadgeProps {
  stock: number;
  showQuantity?: boolean;
}

export function StockBadge({ stock, showQuantity = false }: StockBadgeProps) {
  const variant = getStockStatusBadgeVariant(stock);
  
  const getStatusText = (stock: number) => {
    if (stock === 0) return 'Sem estoque';
    if (stock < 10) return 'Estoque baixo';
    if (stock < 50) return 'Em estoque';
    return 'Bem estocado';
  };

  return (
    <Badge variant={variant}>
      {showQuantity ? `${stock} un.` : getStatusText(stock)}
    </Badge>
  );
}