import { useEffect } from 'react';
import { useAuth } from 'react-oidc-context';
import { setTokenGetter } from '@/api/client';

export function useApiClient() {
  const auth = useAuth();

  useEffect(() => {
    setTokenGetter(() => auth.user?.access_token);
  }, [auth.user]);
}