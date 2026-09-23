import { useAuth } from '@/hooks/useAuth';

export default function Home() {
  const { user, accessToken, isAuthenticated, isLoading, login, logout } = useAuth();

  if (isLoading) return <div style={{ padding: 20 }}>Загрузка Keycloak...</div>;

  if (!isAuthenticated) {
    return (
      <div style={{ padding: 20 }}>
        <h1>mediasphere</h1>
        <button onClick={login}>Войти</button>
      </div>
    );
  }

  return (
    <div style={{ padding: 20 }}>
      <h1>Привет, {user?.preferred_username}</h1>
      <p>Email: {user?.email}</p>

      <h3>Access token</h3>
      <textarea
        readOnly
        value={accessToken}
        style={{ width: '100%', height: 200, fontFamily: 'monospace', fontSize: 12 }}
      />

      <button onClick={logout}>Выйти</button>
    </div>
  );
}