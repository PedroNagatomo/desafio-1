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