import { Button } from '@/components/ui/button';
import { Plus, Filter, Download } from 'lucide-react';

interface PageHeaderProps {
  title: string;
  subtitle?: string;
  showAddButton?: boolean;
  onAddClick?: () => void;
  showFilterButton?: boolean;
  onFilterClick?: () => void;
  showExportButton?: boolean;
  onExportClick?: () => void;
  children?: React.ReactNode;
}

export function PageHeader({
  title,
  subtitle,
  showAddButton = false,
  onAddClick,
  showFilterButton = false,
  onFilterClick,
  showExportButton = false,
  onExportClick,
  children,
}: PageHeaderProps) {
  return (
    <div className="bg-white rounded-lg border border-gray-200 p-6 mb-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-semibold text-gray-900">{title}</h1>
          {subtitle && (
            <p className="text-sm text-gray-500 mt-1">{subtitle}</p>
          )}
        </div>

        <div className="flex items-center space-x-3">
          {showFilterButton && (
            <Button variant="outline" onClick={onFilterClick}>
              <Filter className="w-4 h-4 mr-2" />
              Filtros
            </Button>
          )}

          {showExportButton && (
            <Button variant="outline" onClick={onExportClick}>
              