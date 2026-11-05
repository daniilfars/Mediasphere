import { formatDistanceToNow, format } from 'date-fns';
import { ru } from 'date-fns/locale';
import { useLocation, useParams, Link } from "react-router-dom";
import { useState, useEffect, useRef } from "react";
import { useForm } from "react-hook-form";
import { commentAPI } from "../api/commentAPI";
import { likeAPI } from "../api/likeAPI";
import { useAuth } from '../hooks/useAuth';
import "./PostComments.css";

export default function PostComments() {
  const { id } = useParams();
  const location = useLocation();
  const post = location.state?.post;
  const { isAuthenticated, user, login } = useAuth();

  const [activeMenuCommentId, setActiveMenuCommentId] = useState(null);
  const menuRef = useRef(null);

  const date = new Date(post.createdAt);

  const [comments, setComments] = useState([]);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const pageSize = 12;
  const sentinelRef = useRef(null);

  const { register, handleSubmit, reset } = useForm();

  useEffect(() => {
    let cancelled = false;
    async function load() {
      setIsLoading(true);
      setError(null);
      try {
        const data = await commentAPI.get(id, page, pageSize);
        if (cancelled) return;

        const newComments = data?.comments || []; 
        setComments(prev => page === 1 ? newComments : [...prev, ...newComments]);
        setHasMore(newComments.length === pageSize);
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

  const handleButtonLike = async (commentId, currentIsLiked) => {
    if (!isAuthenticated) {
      login();
      return;
    }

    setComments(prevComments => prevComments.map(comment => {
      if (comment.id === commentId) {
        return {
          ...comment,
          isLiked: !currentIsLiked,
          likes: currentIsLiked ? comment.likes - 1 : comment.likes + 1
        };
      }
      return comment;
    }))
    try {
      if (!currentIsLiked) {
        await likeAPI.create(2, commentId);
      } else {
        await likeAPI.delete(2, commentId);
      }
    } catch {
      setComments(prevComments => prevComments.map(comment => {
        if (comment.id === commentId) {
          return {
            ...comment,
            isLiked: currentIsLiked,
            likes: currentIsLiked ? comment.likes + 1 : comment.likes - 1
          };
        }
        return comment;
      }));
    }
  };

  const onSubmit = async (data) => {
    if (!isAuthenticated) {
      login();
      return;
    }

    try {
      const newComment = await commentAPI.create(id, data.content);
      setComments((prevComments) => [newComment, ...prevComments]);
      reset({ content: "" });
      alert("Сохранено");
    }
    catch {
      alert("Не удалось сохранить");
    }
  }

  useEffect(() => {
    function handleClickOutside(event) {
      if (menuRef.current && !menuRef.current.contains(event.target)) {
        setActiveMenuCommentId(null);
      }
    }

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const toogleMenu = (commentId) => {
    setActiveMenuCommentId(prevId => prevId === commentId ? null : commentId);
  };

  const handleDeleteClick = async (commentId) => {    
    if (window.confirm('Вы уверены, что хотите удалить этот комментарий?')) {
      try {
        await commentAPI.delete(commentId);
        setComments(prevComments => prevComments.filter(comment => comment.id != commentId));
      } catch {
        alert("Не удалось удалить комментарий");
      }
      setActiveMenuCommentId(null);
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
        <div className="post-actions-container comments-post-date-container">
          <time className="post-actions-date" dateTime={post.createdAt} title={format(date, "d MMMM 'в' HH:mm", { locale: ru })}>
            {formatDistanceToNow(new Date(post.createdAt), { addSuffix: true, locale: ru })}
          </time>
        </div>
      </div>
      <hr className="comments-hr"/>
      <section className="post-card-actions comments-card-actions">
        <ul className="comments-list">
          {comments.map(comment => (
            <li key={comment.id} className="comments-list-item">
              <div className="comment-container">
                <Link to={`/profile/${comment.authorId}`} className="comments-authorid">{comment.userName}</Link>
                {isAuthenticated && user?.sub === comment.authorId && (
                <div className="post-card-meatball-container" ref={menuRef}>
                  <button className="post-card-meatball-trigger" onClick={() => toogleMenu(comment.id)} type="button" aria-label="Открыть меню действий" >
                    <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor"><circle cx="5" cy="12" r="2"/><circle cx="12" cy="12" r="2"/><circle cx="19" cy="12" r="2"/></svg>
                  </button>
                  
                  {activeMenuCommentId === comment.id && (
                    <div className="post-card-dropdown">
                      <button className="post-card-dropdown-btn post-card-delete-btn" onClick={() => handleDeleteClick(comment.id)} type="button" >
                        Удалить
                      </button>
                      {/*<Link className="post-card-dropdown-btn" to={`/post/${post.id}/edit`} state={{post}}>Редактировать</Link>*/}
                    </div>
                  )}
                </div>
                )}
              </div>
              <p className="comments-content">{comment.content}</p>
              <button onClick={() => handleButtonLike(comment.id, comment.isLiked)} className={`post-card-btn ${comment.isLiked ? 'is-liked' : ''}`} type="button" aria-label="Поставить лайк">
                <svg className="post-card-like-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z" />
                </svg>
                <span>{comment.likes}</span>
              </button>
            </li>
          ))}
        </ul>
      </section>
      <footer className="comments-footer">
        <form onSubmit={handleSubmit(onSubmit)}>
          <input placeholder="Написать комментарий..." {...register("content", {
            required: "Содержимое комментария не может быть пустым"
          })}/>
          <button type="submit">
            <svg className="post-card-send-icon" viewBox="0 0 24 24" fill="currentColor" stroke="currentColor" xmlns="http://w3.org" style={{ width: '24px', height: '24px' }}>
              <path d="M2.01 21L23 12L2.01 3L2 10L17 12L2 14L2.01 21Z" />
            </svg>
          </button>
        </form>
      </footer>
    </article>
    <div ref={sentinelRef} className="feed-sentinel" />
    {isLoading && <div className="feed-loading">Загрузка...</div>}
    {error && <div className="feed-error">Ошибка: {error}</div>}
  </div>
  );
};