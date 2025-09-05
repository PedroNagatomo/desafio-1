import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { AppLayout } from '@/components/layout/AppLayout';
import { PageHeader } from '@/components/layout/PageHeader';
import { Eye, Edit, Trash2, MoreHorizontal, FolderOpen } from 'lucide-react';
import { Category } from '@/types';
import { categoryService, GetCategoriesParams } from '@/services/categoryService';
import { formatDate } from '@/lib/utils';
import { CategoryForm } from './CategoryForm';
import { CategoryDetails } from './CategoryDetails';
import { useApiMutation } from '@/hooks/useApi';
import { toast } from 'sonner';

export function CategoriesPage() {
  const [params, setParams] = useState<GetCategoriesParams>({
    page: 1,
    pageSize: 20,
  });
  const [selectedCategory, setSelectedCategory] = useState<Category | null>(null);
  const [showCreateDialog, setShowCreateDialog] = useState(false);
  const [showEditDialog, setShowEditDialog] = useState(false);
  const [showDetailsDialog, setShowDetailsDialog] = useState(false);

  // Query para categorias
  const { data: categoriesData, isLoading, refetch } = useQuery({
    queryKey: ['categories', params],
    queryFn: () => categoryService.getCategories(params),
  });

  // Mutation para deletar categoria
  const deleteCategoryMutation = useApiMutation(
    categoryService.deleteCategory,
    {
      onSuccess: () => {
        toast.success('Categoria deletada com sucesso!');
        refetch();
      },
      invalidateQueries: [['categories']],
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

  const handleDeleteCategory = (category: Category) => {
    if (category.productCount > 0) {
      toast.error('Não é possível deletar uma categoria que possui produtos associados.');
      return;
    }

    if (confirm(`Tem certeza que deseja deletar a categoria "${category.name}"?`)) {
      deleteCategoryMutation.mutate(category.id);
    }
  };

  const handleViewCategory = (category: Category) => {
    setSelectedCategory(category);
    setShowDetailsDialog(true);
  };

  const handleEditCategory = (category: Category) => {
    setSelectedCategory(category);
    setShowEditDialog(true);
  };

  const handleFormSuccess = () => {
    setShowCreateDialog(false);
    setShowEditDialog(false);
    setSelectedCategory(null);
    refetch();
  };

  const columns: Column<Category>[] = [
    {
      key: 'name',
      header: 'Categoria',
      render: (category) => (
        <div className="flex items-center space-x-3">
          <div className="w-10 h-10 bg-blue-100 rounded-lg flex items-center justify-center">
            <FolderOpen className="w-5 h-5 text-blue-600" />
          </div>
          <div>
            <div className="font-medium">{category.name}</div>
            {category.description && (
              <div className="text-sm text-gray-500 max-w-xs truncate">
                {category.description}
              </div>
            )}
          </div>
        </div>
      ),
    },
    {
      key: 'productCount',
      header: 'Produtos',
      render: (category) => (
        <Badge variant="secondary">
          {category.productCount} produto{category.productCount !== 1 ? 's' : ''}
        </Badge>
      ),
    },
    {
      key: 'isActive',
      header: 'Status',
      render: (category) => (
        <Badge variant={category.isActive ? 'default' : 'secondary'}>
          {category.isActive ? 'Ativo' : 'Inativo'}
        </Badge>
      ),
    },
    {
      key: 'createdAt',
      header: 'Criado em',
      render: (category) => (
        <span className="text-sm text-gray-500">
          {formatDate(category.createdAt)}
        </span>
      ),
    },
    {
      key: 'actions',
      header: 'Ações',
      render: (category) => (
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" size="sm">
              <MoreHorizontal className="w-4 h-4" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            <DropdownMenuItem onClick={() => handleViewCategory(category)}>
              <Eye className="w-4 h-4 mr-2" />
              Visualizar
            </DropdownMenuItem>
            <DropdownMenuItem onClick={() => handleEditCategory(category)}>
              <Edit className="w-4 h-4 mr-2" />
              Editar
            </DropdownMenuItem>
            <DropdownMenuItem
              onClick={() => handleDeleteCategory(category)}
              className="text-red-600"
              disabled={category.productCount > 0}
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
          title="Categorias"
          subtitle="Organize seus produtos em categorias"
          showAddButton
          onAddClick={() => setShowCreateDialog(true)}
        />

        <DataTable
          data={categoriesData?.data || []}
          columns={columns}
          loading={isLoading}
          pagination={categoriesData}
          onPageChange={handlePageChange}
          onPageSizeChange={handlePageSizeChange}
          onSearch={handleSearch}
          searchPlaceholder="Buscar categorias..."
          emptyTitle="Nenhuma categoria encontrada"
          emptyDescription="Comece criando sua primeira categoria"
          emptyActionLabel="Criar Categoria"
          onEmptyAction={() => setShowCreateDialog(true)}
        />

        {/* Create Category Dialog */}
        <Dialog open={showCreateDialog} onOpenChange={setShowCreateDialog}>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Criar Categoria</DialogTitle>
            </DialogHeader>
            <CategoryForm
              onSuccess={handleFormSuccess}
              onCancel={() => setShowCreateDialog(false)}
            />
          </DialogContent>
        </Dialog>

        {/* Edit Category Dialog */}
        <Dialog open={showEditDialog} onOpenChange={setShowEditDialog}>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Editar Categoria</DialogTitle>
            </DialogHeader>
            {selectedCategory && (
              <CategoryForm
                category={selectedCategory}
                onSuccess={handleFormSuccess}
                onCancel={() => setShowEditDialog(false)}
              />
            )}
          </DialogContent>
        </Dialog>

        {/* Category Details Dialog */}
        <Dialog open={showDetailsDialog} onOpenChange={setShowDetailsDialog}>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle>Detalhes da Categoria</DialogTitle>
            </DialogHeader>
            {selectedCategory && (
              <CategoryDetails
                category={selectedCategory}
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