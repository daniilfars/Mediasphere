import { formatDistanceToNow, format } from 'date-fns';
import { ru } from 'date-fns/locale';
import { Link } from 'react-router-dom';
import {useState, useEffect, useRef } from 'react';
import { useAuth } from '../../hooks/useAuth';
import './PostCard.css';

export default function PostCard({ post, onLikeToggle, onPostDelete }) {
  const date = new Date(post.createdAt);
  const { isAuthenticated, user, login } = useAuth();
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const menuRef = useRef(null);

  useEffect(() => {
    function handleClickOutside(event) {
        if (menuRef.current && !menuRef.current.contains(event.target)) {
            setIsMenuOpen(false);
        }
    }

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const handleButtonLike = async (e) => {
    e.preventDefault();
    e.stopPropagation();

    if (!isAuthenticated) {
        login();
        return;
    }

    onLikeToggle(post.id, post.isLiked);
  };

  const handleDeleteClick = (e) => {
    e.preventDefault();
    e.stopPropagation();
    
    if (window.confirm('Вы уверены, что хотите удалить этот пост?')) {
        onPostDelete(post.id);
        setIsMenuOpen(false);
    }
  };

  return (
    <article className="post-card">
        <header className="post-card-header">
            <Link className="post-card-link" to={`/profile/${post.authorId}`}>{post.userName}</Link>

            {isAuthenticated && user?.sub === post.authorId && (
                <div className="post-card-meatball-container" ref={menuRef}>
                    <button className="post-card-meatball-trigger" onClick={() => setIsMenuOpen(!isMenuOpen)} type="button" aria-label="Открыть меню действий" >
                        <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor"><circle cx="5" cy="12" r="2"/><circle cx="12" cy="12" r="2"/><circle cx="19" cy="12" r="2"/></svg>
                    </button>
                    
                    {isMenuOpen && (
                        <div className="post-card-dropdown">
                            <button className="post-card-dropdown-btn post-card-delete-btn" onClick={handleDeleteClick} type="button" >
                                Удалить
                            </button>
                            <Link className="post-card-dropdown-btn" to={`/post/${post.id}/edit`} state={{post}}>Редактировать</Link>
                        </div>
                    )}
                </div>
            )}
        </header>
        <div className="post-card-body">
            {post.imageUrl && <img src={post.imageUrl} alt="Картинка поста" />}
            <p className="post-card-content">{post.content}</p>
        </div>
        <footer className="post-card-actions">
            <div className="post-actions-container">
                <div className="post-actions-item">
                    <button onClick={handleButtonLike} className={`post-card-like ${post.isLiked ? 'is-liked' : ''}`} type="button" aria-label="Поставить лайк">
                        <svg className="post-card-like-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                            <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z" />
                        </svg>
                        <span>{post.likes}</span>
                    </button>
                </div>
                <div className="post-actions-item">
                    <button className="post-card-like" type="button" aria-label="Комментарии">
                        <svg className="post-card-like-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                            <path d="M21 11.5a8.38 8.38 0 0 1-.9 3.8 8.5 8.5 0 0 1-7.6 4.7 8.38 8.38 0 0 1-3.8-.9L3 21l1.9-5.7a8.38 8.38 0 0 1-.9-3.8 8.5 8.5 0 0 1 4.7-7.6 8.38 8.38 0 0 1 3.8-.9h.5a8.48 8.48 0 0 1 8 8v.5z" />
                        </svg>
                        <span>{post.comments}</span>
                    </button>
                </div>
            </div>
            <div className="post-actions-container">
                <time className="post-actions-date" dateTime={post.createdAt} title={format(date, "d MMMM 'в' HH:mm", { locale: ru })}>
                    {formatDistanceToNow(new Date(post.createdAt), { addSuffix: true, locale: ru })}
                </time>
            </div>
        </footer>
    </article>
  );

}