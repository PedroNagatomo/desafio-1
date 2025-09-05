import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useQuery } from '@tanstack/react-query';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { LoadingSpinner } from '@/components/common/LoadingSpinner';
import { Product, CreateProductDto, UpdateProductDto } from '@/types';
import { productService } from '@/services/productService';
import { categoryService } from '@/services/categoryService';
import { useApiMutation } from '@/hooks/useApi';
import { toast } from 'sonner';

const productSchema = z.object({
  name: z.string().min(1, 'Nome é obrigatório').max(200, 'Nome muito longo'),
  description: z.string().optional(),
  price: z.number().min(0.01, 'Preço deve ser maior que zero'),
  categoryId: z.string().min(1, 'Categoria é obrigatória'),
  stock: z.number().int().min(0, 'Estoque não pode ser negativo'),
  sku: z.string().optional(),
});

type ProductFormData = z.infer<typeof productSchema>;

interface ProductFormProps {
  product?: Product;
  onSuccess: () => void;
  onCancel: () => void;
}

export function ProductForm({ product, onSuccess, onCancel }: ProductFormProps) {
  const isEdit = !!product;

  const {
    register,
    handleSubmit,
    setValue,
    watch,
    formState: { errors, isSubmitting },
  } = useForm<ProductFormData>({
    resolver: zodResolver(productSchema),
    defaultValues: {
      name: product?.name || '',
      description: product?.description || '',
      price: product?.price || 0,
      categoryId: product?.categoryId || '',
      stock: product?.stock || 0,
      sku: product?.sku || '',
    },
  });

  // Query para categorias ativas
  const { data: categories, isLoading: categoriesLoading } = useQuery({
    queryKey: ['categories', 'active'],
    queryFn: () => categoryService.getActiveCategories(),
  });

  // Mutations
  const createProductMutation = useApiMutation(productService.createProduct, {
    onSuccess: () => {
      toast.success('Produto criado com sucesso!');
      onSuccess();
    },
  });

  const updateProductMutation = useApiMutation(
    ({ id, data }: { id: string; data: UpdateProductDto }) =>
      productService.updateProduct(id, data),
    {
      onSuccess: () => {
        toast.success('Produto atualizado com sucesso!');
        onSuccess();
      },
    }
  );

  const onSubmit = async (data: ProductFormData) => {
    if (isEdit && product) {
      const updateData: UpdateProductDto = {
        name: data.name,
        description: data.description,
        price: data.price,
        categoryId: data.categoryId,
        sku: data.sku,
      };
      updateProductMutation.mutate({ id: product.id, data: updateData });
    } else {
      const createData: CreateProductDto = {
        name: data.name,
        description: data.description,
        price: data.price,
        categoryId: data.categoryId,
        stock: data.stock,
        sku: data.sku,
      };
      createProductMutation.mutate(createData);
    }
  };

  const selectedCategoryId = watch('categoryId');

  if (categoriesLoading) {
    return (
      <div className="flex items-center justify-center p-8">
        <LoadingSpinner size="lg" />
      </div>
    );
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {/* Nome */}
        <div className="md:col-span-2">
          <Label htmlFor="name">Nome do Produto *</Label>
          <Input
            id="name"
            {...register('name')}
            placeholder="Digite o nome do produto"
            className={errors.name ? 'border-red-500' : ''}
          />
          {errors.name && (
            <p className="text-sm text-red-500 mt-1">{errors.name.message}</p>
          )}
        </div>

        {/* SKU */}
        <div>
          <Label htmlFor="sku">SKU</Label>
          <Input
            id="sku"
            {...register('sku')}
            placeholder="SKU do produto (opcional)"
            className={errors.sku ? 'border-red-500' : ''}
          />
          {errors.sku && (
            <p className="text-sm text-red-500 mt-1">{errors.sku.message}</p>
          )}
        </div>

        {/* Categoria */}
        <div>
          <Label htmlFor="categoryId">Categoria *</Label>
          <Select
            value={selectedCategoryId}
            onValueChange={(value) => setValue('categoryId', value)}
          >
            <SelectTrigger className={errors.categoryId ? 'border-red-500' : ''}>
              <SelectValue placeholder="Selecione uma categoria" />
            </SelectTrigger>
            <SelectContent>
              {categories?.map((category) => (
                <SelectItem key={category.id} value={category.id}>
                  {category.name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
          {errors.categoryId && (
            <p className="text-sm text-red-500 mt-1">{errors.categoryId.message}</p>
          )}
        </div>

        {/* Preço */}
        <div>
          <Label htmlFor="price">Preço (R$) *</Label>
          <Input
            id="price"
            type="number"
            step="0.01"
            {...register('price', { valueAsNumber: true })}
            placeholder="0,00"
            className={errors.price ? 'border-red-500' : ''}
          />
          {errors.price && (
            <p className="text-sm text-red-500 mt-1">{errors.price.message}</p>
          )}
        </div>

        {/* Estoque (apenas na criação) */}
        {!isEdit && (
          <div>
            <Label htmlFor="stock">Estoque Inicial *</Label>
            <Input
              id="stock"
              type="number"
              {...register('stock', { valueAsNumber: true })}
              placeholder="0"
              className={errors.stock ? 'border-red-500' : ''}
            />
            {errors.stock && (
              <p className="text-sm text-red-500 mt-1">{errors.stock.message}</p>
            )}
          </div>
        )}
      </div>

      {/* Descrição */}
      <div>
        <Label htmlFor="description">Descrição</Label>
        <Textarea
          id="description"
          {...register('description')}
          placeholder="Descrição detalhada do produto (opcional)"
          rows={4}
          className={errors.description ? 'border-red-500' : ''}
        />
        {errors.description && (
          <p className="text-sm text-red-500 mt-1">{errors.description.message}</p>
        )}
      </div>

      {/* Actions */}
      <div className="flex justify-end space-x-3 pt-6 border-t">
        <Button type="button" variant="outline" onClick={onCancel}>
          Cancelar
        </Button>
        <Button
          type="submit"
          disabled={isSubmitting || createProductMutation.isPending || updateProductMutation.isPending}
        >
          {(isSubmitting || createProductMutation.isPending || updateProductMutation.isPending) && (
            <LoadingSpinner size="sm" className="mr-2" />
          )}
          {isEdit ? 'Atualizar' : 'Criar'} Produto
        </Button>
      </div>
    </form>
  );
}
