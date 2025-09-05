import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Category, CreateCategoryDto, UpdateCategoryDto } from '@/types';
import { categoryService } from '@/services/categoryService';
import { useApiMutation } from '@/hooks/useApi';
import { toast } from 'sonner';

const categorySchema = z.object({
  name: z.string().min(1, 'Nome é obrigatório').max(100, 'Nome muito longo'),
  description: z.string().max(500, 'Descrição muito longa').optional(),
});

type CategoryFormData = z.infer<typeof categorySchema>;

interface CategoryFormProps {
  category?: Category;
  onSuccess: () => void;
  onCancel: () => void;
}

export function CategoryForm({ category, onSuccess, onCancel }: CategoryFormProps) {
  const isEdit = !!category;

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<CategoryFormData>({
    resolver: zodResolver(categorySchema),
    defaultValues: {
      name: category?.name || '',
      description: category?.description || '',
    },
  });

  // Mutations
  const createCategoryMutation = useApiMutation(categoryService.createCategory, {
    onSuccess: () => {
      toast.success('Categoria criada com sucesso!');
      onSuccess();
    },
  });

  const updateCategoryMutation = useApiMutation(
    ({ id, data }: { id: string; data: UpdateCategoryDto }) =>
      categoryService.updateCategory(id, data),
    {
      onSuccess: () => {
        toast.success('Categoria atualizada com sucesso!');
        onSuccess();
      },
    }
  );

  const onSubmit = async (data: CategoryFormData) => {
    if (isEdit && category) {
      const updateData: UpdateCategoryDto = {
        name: data.name,
        description: data.description,
      };
      updateCategoryMutation.mutate({ id: category.id, data: updateData });
    } else {
      const createData: CreateCategoryDto = {
        name: data.name,
        description: data.description,
      };
      createCategoryMutation.mutate(createData);
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      {/* Nome */}
      <div>
        <Label htmlFor="name">Nome da Categoria *</Label>
        <Input
          id="name"
          {...register('name')}
          placeholder="Digite o nome da categoria"
          className={errors.name ? 'border-red-500' : ''}
        />
        {errors.name && (
          <p className="text-sm text-red-500 mt-1">{errors.name.message}</p>
        )}
      </div>

      {/* Descrição */}
      <div>
        <Label htmlFor="description">Descrição</Label>
        <Textarea
          id="description"
          {...register('description')}
          placeholder="Descrição da categoria (opcional)"
          rows={3}
          className={errors.description ? 'border-red-500' : ''}
        />
        {errors.description && (
          <p className="text-sm text-red-500 mt-1">{errors.description.message}</p>
        )}
      </div>

      {/* Actions */}
      <div className="flex justify-end space-x-3 pt-4 border-t">
        <Button type="button" variant="outline" onClick={onCancel}>
          Cancelar
        </Button>
        <Button
          type="submit"
          disabled={isSubmitting || createCategoryMutation.isPending || updateCategoryMutation.isPending}
        >
          {(isSubmitting || createCategoryMutation.isPending || updateCategoryMutation.isPending) && (
            <LoadingSpinner size="sm" className="mr-2" />
          )}
          {isEdit ? 'Atualizar' : 'Criar'} Categoria
        </Button>
      </div>
    </form>
  );
}
