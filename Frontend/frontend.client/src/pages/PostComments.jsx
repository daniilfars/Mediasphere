import { formatDistanceToNow, format } from 'date-fns';
import { ru } from 'date-fns/locale';
import { useLocation, useParams, Link } from "react-router-dom";
import { useState, useEffect, useRef } from "react";
import { commentAPI } from "../api/commentAPI";
import { useAuth } from '../hooks/useAuth';
import "./PostComments.css";

export default function PostComments() {
  const { id } = useParams();
  const location = useLocation();
  const post = location.state?.post;
  const { isAuthenticated, login } = useAuth();

  const date = new Date(post.createdAt);

  const [ comments, setComments ] = useState([]);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const pageSize = 12;
  const sentinelRef = useRef(null);

  useEffect(() => {
    let cancelled = false;
    async function load() {
      setIsLoading(true);
      setError(null);
      try {
        const data = await commentAPI.get(id, page, pageSize);
        if (cancelled) return;
        setComments(prev => page === 1 ? data.comments : [...prev, ...data.comments]);
        setHasMore(data.comments.length === pageSize);
      } catch (err) {
        if (cancelled) return;
        setError(err.message || 'Ошибка загрузки');
      } finally {
        if (!cancelled) setIsLoading(false);
      }
    }
    load();
    return () => { cancelled = true; };
  }, [page, id]);

  useEffect(() => {
    const sentinel = sentinelRef.current;
    if (!sentinel || !hasMore || isLoading) return;
    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting) {
          setPage(p => p + 1);
        }
      },
      { rootMargin: '200px' }
    );
    observer.observe(sentinel);
    return () => observer.disconnect();
  }, [hasMore, isLoading]);

  const handleButtonLike = async (e) => {
    e.preventDefault();
    e.stopPropagation();

    if (!isAuthenticated) {
        login();
        return;
    }
  };

  return (
  <div className="container feed">
    <article className="post-card">
      <header className="post-card-header">
        <Link className="post-card-link" to={`/profile/${post.authorId}`}>{post.userName}</Link>
      </header>
      <div className="post-card-body">
        {post.imageUrl && <img src={post.imageUrl} alt="Картинка поста" />}
        <p className="post-card-content">{post.content}</p>
        <div className="post-actions-container">
          <time className="post-actions-date" dateTime={post.createdAt} title={format(date, "d MMMM 'в' HH:mm", { locale: ru })}>
            {formatDistanceToNow(new Date(post.createdAt), { addSuffix: true, locale: ru })}
          </time>
        </div>
      </div>
      <hr />
      <footer className="post-card-actions">
        <ul className="comments-list">
          {comments.map(comment => (
            <li key={comment.id} className="comments-list-item">
              <Link to={`/profile/${post.authorId}`} className="comments-authorid">{comment.userName}</Link>
              <p className="comments-content">{comment.content}</p>
              <button onClick={handleButtonLike} className={`post-card-btn ${post.isLiked ? 'is-liked' : ''}`} type="button" aria-label="Поставить лайк">
                <svg className="post-card-like-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z" />
                </svg>
                <span>{comment.likes}</span>
              </button>
            </li>
          ))}
        </ul>
        {/*Добавить создание комментария */}
      </footer>
    </article>
    <div ref={sentinelRef} className="feed-sentinel" />
    {isLoading && <div className="feed-loading">Загрузка...</div>}
    {error && <div className="feed-error">Ошибка: {error}</div>}
    {!hasMore && comments.length > 0 && (
        <div className="feed-end">Комментариев больше нет</div>
    )}
    {!isLoading && comments.length === 0 && (
    <div className="feed-empty">Комментариев нету</div>
    )}
  </div>
  );
};