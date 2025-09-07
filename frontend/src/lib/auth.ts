import Keycloak from 'keycloak-js';

const keycloakConfig = {
  url: import.meta.env.VITE_KEYCLOAK_URL || 'http://localhost:8080',
  realm: import.meta.env.VITE_KEYCLOAK_REALM || 'hypesoft',
  clientId: import.meta.env.VITE_KEYCLOAK_CLIENT_ID || 'hypesoft-frontend',
};

interface UserInfo {
  id?: string;
  username?: string;
  email?: string;
  firstName?: string;
  lastName?: string;
  roles?: string[];
}

class KeycloakService {
  private keycloak: Keycloak;

  constructor() {
    this.keycloak = new Keycloak(keycloakConfig);
  }

  async init(): Promise<boolean> {
    try {
      const authenticated = await this.keycloak.init({
        onLoad: 'check-sso',
        silentCheckSsoRedirectUri: window.location.origin + '/silent-check-sso.html',
        pkceMethod: 'S256',
        checkLoginIframe: false,
      });

      if (authenticated) {
        this.setupTokenRefresh();
      }

      return authenticated;
    } catch (error) {
      console.error('Keycloak initialization failed:', error);
      return false;
    }
  }

  login() {
    return this.keycloak.login({
      redirectUri: window.location.origin,
    });
  }

  logout() {
    return this.keycloak.logout({
      redirectUri: window.location.origin,
    });
  }

  getToken(): string | undefined {
    return this.keycloak.token;
  }

  isAuthenticated(): boolean {
    return !!this.keycloak.authenticated;
  }

  hasRole(role: string): boolean {
    return this.keycloak.hasRealmRole(role);
  }

  hasAnyRole(roles: string[]): boolean {
    return roles.some(role => this.hasRole(role));
  }

  getUserInfo(): UserInfo | null {
    if (!this.keycloak.authenticated) return null;

    const tokenParsed = this.keycloak.tokenParsed;
    if (!tokenParsed) return null;

    return {
      id: tokenParsed.sub,
      username: tokenParsed.preferred_username,
      email: tokenParsed.email,
      firstName: tokenParsed.given_name,
      lastName: tokenParsed.family_name,
      roles: tokenParsed.realm_access?.roles || [],
    };
  }

  isTokenExpired(): boolean {
    return this.keycloak.isTokenExpired();
  }

  async refreshToken(): Promise<boolean> {
    try {
      const refreshed = await this.keycloak.updateToken(30);
      return refreshed;
    } catch (error) {
      console.error('Token refresh failed:', error);
      return false;
    }
  }

  private setupTokenRefresh() {
    // Refresh token every 4 minutes
    setInterval(async () => {
      try {
        await this.keycloak.updateToken(70);
      } catch (error) {
        console.error('Token refresh failed:', error);
        this.login();
      }
    }, 240000); // 4 minutes
  }

  getKeycloakInstance() {
    return this.keycloak;
  }
}

export const keycloakService = new KeycloakService();