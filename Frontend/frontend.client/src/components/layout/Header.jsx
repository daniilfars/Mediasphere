import { Link } from 'react-router-dom';
import logo from '@/assets/logo.png';
import { useAuth } from '@/hooks/useAuth'
import './Header.css';

export default function Header() {
  const { isAuthenticated, login } = useAuth();

  return (
    <header className="header">
      <div className="container header-container">
        <div className="header-container-item">
          <Link to="/" className="header-link header-logo">
            <img src={logo} alt="Mediasphere" />
            <span className="header-logo-text">ediasphere</span>
          </Link>
        </div>
        <div className="header-container-item">
          {isAuthenticated ? (
            <Link to="/profile" className="header-link">
              <span className="header-link-text">Профиль</span>
            </Link>
          ) : (
            <button type="button" onClick={login} className="header-link header-link-button">
              <span className="header-link-text">Войти</span>
            </button>
          )
          
          }
        </div>
      </div>
    </header>
  );
}