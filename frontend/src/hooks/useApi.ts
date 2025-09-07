import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';

interface ApiError {
  error: string;
  details?: Array<{
    propertyName: string;
    errorMessage: string;
  }>;
  statusCode: number;
}

export function useApiQuery<T>(
  key: string[],
  queryFn: () => Promise<T>,
  options?: {
    enabled?: boolean;
    staleTime?: number;
    gcTime?: number;
  }
) {
  return useQuery({
    queryKey: key,
    queryFn,
    ...options,
    retry: (failureCount, error: any) => {
      if (error?.response?.status >= 400 && error?.response?.status < 500) {
        return false;
      }
      return failureCount < 2;
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
        toast.error(apiError?.error || error.message || 'Erro na operação');
      }
      options?.onError?.(error, variables);
    },
  });
}
