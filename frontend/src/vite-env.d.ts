interface ImportMetaEnv {
  VITE_KEYCLOAK_CLIENT_ID: string
  VITE_KEYCLOAK_REALM: string
  VITE_KEYCLOAK_URL: string
  readonly VITE_API_BASE_URL: string
  // more env variables...
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}