import React, { createContext, useContext, useEffect, useState, ReactNode } from 'react';
import { authService, AuthState, User } from '@/services/authService';
import { LoadingSpinner } from '@/components/common/LoadingSpinner';

interface AuthContextType extends AuthState {
  login: () => void;
  logout: () => void;
  hasRole: (role: string) => boolean;
  hasAnyRole: (roles: string[]) => boolean;
  isAdmin: () => boolean;
  isManager: () => boolean;
  isUser: () => boolean;
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
  children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [authState, setAuthState] = useState<AuthState>({
    isAuthenticated: false,
    isLoading: true,
    user: null,
    token: null,
  });

  useEffect(() => {
    let mounted = true;

    const initAuth = async () => {
      try {
        await authService.initialize();
        
        if (mounted) {
          const newAuthState = authService.getAuthState();
          setAuthState(newAuthState);
        }
      } catch (error) {
        console.error('Auth initialization failed:', error);
        if (mounted) {
          setAuthState(prev => ({ ...prev, isLoading: false }));
        }
      }
    };

    initAuth();

    return () => {
      mounted = false;
    };
  }, []);

  const login = () => {
    authService.login();
  };

  const logout = () => {
    authService.logout();
  };

  const contextValue: AuthContextType = {
    ...authState,
    login,
    logout,
    hasRole: authService.hasRole.bind(authService),
    hasAnyRole: authService.hasAnyRole.bind(authService),
    isAdmin: authService.isAdmin.bind(authService),
    isManager: authService.isManager.bind(authService),
    isUser: authService.isUser.bind(authService),
  };

  if (authState.isLoading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <LoadingSpinner size="lg" className="mb-4" />
          <p className="text-gray-600">Inicializando aplicação...</p>
        </div>
      </div>
    );
  }

  return (
    <AuthContext.Provider value={contextValue}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}
