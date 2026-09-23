import { useAuth as useOidcAuth } from 'react-oidc-context';

export function useAuth() {
  const auth = useOidcAuth();

  return {
    user: auth.user?.profile ?? null,
    accessToken: auth.user?.access_token ?? null,
    isAuthenticated: auth.isAuthenticated,
    isLoading: auth.isLoading,
    error: auth.error,
    login: () => auth.signinRedirect(),
    logout: () => auth.signoutRedirect(),
  };
}