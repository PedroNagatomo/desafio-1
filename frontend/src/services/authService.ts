import { keycloakService } from '@/lib/auth';

export interface User {
  id: string;
  username: string;
  email: string;
  firstName?: string;
  lastName?: string;
  roles: string[];
}

export interface AuthState {
  isAuthenticated: boolean;
  isLoading: boolean;
  user: User | null;
  token: string | null;
}

export const authService = {
  async initialize(): Promise<boolean> {
    return await keycloakService.init();
  },

  login() {
    return keycloakService.login();
  },

  logout() {
    return keycloakService.logout();
  },

  getAuthState(): AuthState {
    const isAuthenticated = keycloakService.isAuthenticated();
    const userInfo = keycloakService.getUserInfo();
    const token = keycloakService.getToken() || null;

    // Converter userInfo para o tipo User correto ou null
    const user: User | null = userInfo && userInfo.id ? {
      id: userInfo.id,
      username: userInfo.username || '',
      email: userInfo.email || '',
      firstName: userInfo.firstName,
      lastName: userInfo.lastName,
      roles: userInfo.roles || [],
    } : null;

    return {
      isAuthenticated,
      isLoading: false,
      user,
      token,
    };
  },

  getToken(): string | null {
    return keycloakService.getToken() || null;
  },

  hasRole(role: string): boolean {
    return keycloakService.hasRole(role);
  },

  hasAnyRole(roles: string[]): boolean {
    return keycloakService.hasAnyRole(roles);
  },

  async refreshToken(): Promise<boolean> {
    return await keycloakService.refreshToken();
  },

  // Role helpers
  isAdmin(): boolean {
    return this.hasRole('admin');
  },

  isManager(): boolean {
    return this.hasAnyRole(['admin', 'manager']);
  },

  isUser(): boolean {
    return this.hasAnyRole(['admin', 'manager', 'user']);
  },
};
