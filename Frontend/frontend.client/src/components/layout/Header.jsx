import { Link } from 'react-router-dom';
import logo from '@/assets/logo.png';
import './Header.css';

export default function Header() {
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
          <Link to="/profile" className="header-link">
            <span className="header-link-text">Профиль</span>
          </Link>
        </div>
      </div>
    </header>
  );
}