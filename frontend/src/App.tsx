import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { Toaster } from 'sonner';
import { Dashboard } from '@/pages/dashboard/Dashboard';
import { ProductsPage } from '@/pages/products/ProductsPage';
import { CategoriesPage } from '@/pages/categories/CategoriesPage';
import './index.css';

// Configuração do React Query
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000, // 5 minutos
      cacheTime: 10 * 60 * 1000, // 10 minutos
      retry: 1,
      refetchOnWindowFocus: false,
    },
    mutations: {
      retry: 0,
    },
  },
});

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <Router>
        <div className="min-h-screen bg-gray-50">
          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/products" element={<ProductsPage />} />
            <Route path="/categories" element={<CategoriesPage />} />
            
            {/* Rotas futuras */}
            <Route path="/reports" element={
              <div className="flex items-center justify-center h-screen">
                <div className="text-center">
                  <h2 className="text-2xl font-bold text-gray-900">Relatórios</h2>
                  <p className="text-gray-500 mt-2">Em desenvolvimento...</p>
                </div>
              </div>
            } />
            
            <Route path="/users" element={
              <div className="flex items-center justify-center h-screen">
                <div className="text-center">
                  <h2 className="text-2xl font-bold text-gray-900">Usuários</h2>
                  <p className="text-gray-500 mt-2">Em desenvolvimento...</p>
                </div>
              </div>
            } />
            
            <Route path="/settings" element={
              <div className="flex items-center justify-center h-screen">
                <div className="text-center">
                  <h2 className="text-2xl font-bold text-gray-900">Configurações</h2>
                  <p className="text-gray-500 mt-2">Em desenvolvimento...</p>
                </div>
              </div>
            } />

            {/* 404 Page */}
            <Route path="*" element={
              <div className="flex items-center justify-center h-screen">
                <div className="text-center">
                  <h2 className="text-2xl font-bold text-gray-900">Página não encontrada</h2>
                  <p className="text-gray-500 mt-2">A página que você está procurando não existe.</p>
                </div>
              </div>
            } />
          </Routes>
        </div>

        {/* Toast Notifications */}
        <Toaster
          position="top-right"
          toastOptions={{
            duration: 4000,
            style: {
              background: 'white',
              color: 'black',
              border: '1px solid #e5e7eb',
            },
          }}
        />

        {/* React Query DevTools (apenas em desenvolvimento) */}
        <ReactQueryDevtools initialIsOpen={false} />
      </Router>
    </QueryClientProvider>
  );
}

export default App;