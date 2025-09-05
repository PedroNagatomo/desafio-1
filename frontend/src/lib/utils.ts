import { type ClassValue, clsx } from "clsx"
import { twMerge } from "tailwind-merge"

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

export function formatCurrency(value: number, currency: string = 'BRL'): string {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: currency,
  }).format(value);
}

export function formatDate(date: string): string {
  return new Intl.DateTimeFormat('pt-BR', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  }).format(new Date(date));
}

export function formatDateTime(date: string): string {
  return new Intl.DateTimeFormat('pt-BR', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  }).format(new Date(date));
}

export function truncateText(text: string, maxLength: number): string {
  if (text.length <= maxLength) return text;
  return text.substring(0, maxLength) + '...';
}

export function getStockStatusColor(stock: number): string {
  if (stock === 0) return 'text-red-600';
  if (stock < 10) return 'text-yellow-600';
  if (stock < 50) return 'text-blue-600';
  return 'text-green-600';
}

export function getStockStatusBadgeVariant(stock: number): 'destructive' | 'outline' | 'secondary' | 'default' {
  if (stock === 0) return 'destructive';
  if (stock < 10) return 'outline';
  if (stock < 50) return 'secondary';
  return 'default';
}

// ===== src/hooks/useApi.ts =====
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { ApiError } from '@/types';

export function useApiQuery<T>(
  key: string[],
  queryFn: () => Promise<T>,
  options?: {
    enabled?: boolean;
    staleTime?: number;
    cacheTime?: number;
  }
) {
  return useQuery({
    queryKey: key,
    queryFn,
    ...options,
    onError: (error: any) => {
      const apiError = error.response?.data as ApiError;
      toast.error(apiError?.error || 'Erro ao carregar dados');
    },
  });
}

export function useApiMutation<T, V>(
  mutationFn: (variables: V) => Promise<T>,
  options?: {
    onSuccess?: (data: T, variables: V) => void;
    onError?: (error: any, variables: V) => void;
    invalidateQueries?: string[][];
  }
) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn,
    onSuccess: (data, variables) => {
      options?.onSuccess?.(data, variables);
      if (options?.invalidateQueries) {
        options.invalidateQueries.forEach(queryKey => {
          queryClient.invalidateQueries({ queryKey });
        });
      }
    },
    onError: (error: any, variables) => {
      const apiError = error.response?.data as ApiError;
      if (apiError?.details) {
        apiError.details.forEach(detail => {
          toast.error(`${detail.propertyName}: ${detail.errorMessage}`);
        });
      } else {
        toast.error(apiError?.error || 'Erro na operação');
      }
      options?.onError?.(error, variables);
    },
  });
}