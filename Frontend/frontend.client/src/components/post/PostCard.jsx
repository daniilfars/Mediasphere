import { formatDistanceToNow, format } from 'date-fns';
import { ru } from 'date-fns/locale';
import { Link } from 'react-router-dom';
import './PostCard.css';

export default function PostCard({ post }) {
  const date = new Date(post.createdAt);

  return (
    <article className="post-card">
        <header className="post-card-header">
            <Link to={`/profile/${post.authorId}`}>{post.userName}</Link>
        </header>
        <div className="post-card-body">
            {post.imageUrl && <img src={post.imageUrl} alt="" />}
            <p className="post-card-content">{post.content}</p>
        </div>
        <footer className="post-card-actions">
            <div className="post-actions-container">
                <div className="post-actions-item">
                    <button className={`post-card-like ${post.isLiked ? 'is-liked' : ''}`} type="button" aria-label="Поставить лайк">
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
                        <span>{post.likes}</span>
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