import { Edit, FolderOpen, Package, Calendar } from 'lucide-react';
import { Category } from '@/types';
import { formatDateTime } from '@/lib/utils';

interface CategoryDetailsProps {
  category: Category;
  onEdit: () => void;
  onClose: () => void;
}

export function CategoryDetails({ category, onEdit, onClose }: CategoryDetailsProps) {
  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-start justify-between">
        <div className="flex items-start space-x-4">
          <div className="w-16 h-16 bg-blue-100 rounded-lg flex items-center justify-center">
            <FolderOpen className="w-8 h-8 text-blue-600" />
          </div>
          <div>
            <h2 className="text-2xl font-bold">{category.name}</h2>
            <div className="flex items-center space-x-2 mt-2">
              <Badge variant={category.isActive ? 'default' : 'secondary'}>
                {category.isActive ? 'Ativo' : 'Inativo'}
              </Badge>
              <Badge variant="secondary">
                {category.productCount} produto{category.productCount !== 1 ? 's' : ''}
              </Badge>
            </div>
          </div>
        </div>
        <Button onClick={onEdit}>
          <Edit className="w-4 h-4 mr-2" />
          Editar
        </Button>
      </div>

      {/* Stats Card */}
      <Card>
        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle className="text-sm font-medium">Produtos Associados</CardTitle>
          <Package className="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div className="text-2xl font-bold">{category.productCount}</div>
          <p className="text-xs text-muted-foreground">
            {category.productCount === 0 ? 'Nenhum produto associado' : 
             category.productCount === 1 ? '1 produto nesta categoria' : 
             `${category.productCount} produtos nesta categoria`}
          </p>
        </CardContent>
      </Card>

      {/* Descrição */}
      {category.description && (
        <Card>
          <CardHeader>
            <CardTitle>Descrição</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-gray-700">{category.description}</p>
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
              <dd className="text-sm text-gray-900">{formatDateTime(category.createdAt)}</dd>
            </div>
            <div>
              <dt className="text-sm font-medium text-gray-500">Última atualização</dt>
              <dd className="text-sm text-gray-900">{formatDateTime(category.updatedAt)}</dd>
            </div>
            <div>
              <dt className="text-sm font-medium text-gray-500">Status</dt>
              <dd className="text-sm text-gray-900">
                {category.isActive ? 'Ativa' : 'Inativa'}
              </dd>
            </div>
            <div>
              <dt className="text-sm font-medium text-gray-500">ID</dt>
              <dd className="text-sm text-gray-900 font-mono">{category.id}</dd>
            </div>
          </dl>
        </CardContent>
      </Card>

      {/* Actions */}
      <div className="flex justify-end pt-6 border-t">
        <Button variant="outline" onClick={onClose}>
          Fechar
        </Button>
      </div>
    </div>
  );
}