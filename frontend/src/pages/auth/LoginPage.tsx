import { useEffect } from 'react';
import { useAuth } from '@/hooks/useAuth';
import { useLocation, useNavigate } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Package, LogIn } from 'lucide-react';

export function LoginPage() {
  const { isAuthenticated, login } = useAuth();
  const location = useLocation();
  const navigate = useNavigate();

  const from = (location.state as any)?.from?.pathname || '/';

  useEffect(() => {
    if (isAuthenticated) {
      navigate(from, { replace: true });
    }
  }, [isAuthenticated, navigate, from]);

  if (isAuthenticated) {
    return null;
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 bg-blue-600 rounded-full mb-4">
            <Package className="w-8 h-8 text-white" />
          </div>
          <h1 className="text-3xl font-bold text-gray-900">Hypesoft</h1>
          <p className="text-gray-600">Sistema de Gestão de Produtos</p>
        </div>

        <Card>
          <CardHeader className="text-center">
            <CardTitle>Fazer Login</CardTitle>
            <CardDescription>
              Faça login para acessar o sistema de gestão
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Button 
              onClick={login} 
              className="w-full"
              size="lg"
            >
              <LogIn className="w-4 h-4 mr-2" />
              Entrar com Keycloak
            </Button>
            
            <div className="mt-6 text-center">
              <p className="text-sm text-gray-600 mb-2">Usuários de teste:</p>
              <div className="space-y-1 text-xs text-gray-500">
                <div><strong>Admin:</strong> admin / admin123</div>
                <div><strong>Manager:</strong> manager / manager123</div>
                <div><strong>User:</strong> user / user123</div>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}