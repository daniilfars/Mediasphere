import { useApiClient } from '@/hooks/useApiClient';
import AppRouter from '@/router/AppRouter';
import Header from '@/components/layout/Header';
import Footer from '@/components/layout/Footer';
import './App.css';
import { useAuth } from 'react-oidc-context';

function App() {
  const auth = useAuth();
  useApiClient();

  if (auth.isLoading) return <div>Загрузка...</div>;

  return (
    <>
      <Header />
      <main style={{ flex: 1 }}>
        <AppRouter />
      </main>
      <Footer />
    </>
  );
}

export default App;