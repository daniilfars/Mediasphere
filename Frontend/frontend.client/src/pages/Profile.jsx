import { Link, useParams } from 'react-router-dom';
import { userAPI } from '../api/userAPI';
import { followAPI } from '../api/followAPI';
import { postAPI } from '../api/postAPI';
import { likeAPI } from '../api/likeAPI';
import { useState, useEffect, useRef } from 'react';
import PostCard from "@/components/post/PostCard";
import './Profile.css';

export default function Profile() {
  const [user, setUser] = useState(null);
  const [followers, setFollowers] = useState(null);
  const [followings, setFollowings] = useState(null);

  const [isUserLoading, setIsUserLoading] = useState(true);
  const [isFollowersLoading, setIsFollowersLoading] = useState(true);
  const [isFollowingsLoading, setIsFollowingsLoading] = useState(true);
  const [followError, setIsfollowError] = useState(false);

  const { id } = useParams();

  const [posts, setPosts] = useState([]);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const pageSize = 12;
  const sentinelRef = useRef(null);

  const handleLikeToggle = async (postId, currentIsLiked) => {
    setPosts(prevPosts => prevPosts.map(post => {
      if (post.id === postId) {
          return {
              ...post,
              isLiked: !currentIsLiked,
              likes: currentIsLiked ? post.likes - 1 : post.likes + 1
          };
      }
      return post;
    }));
    try {
      if (!currentIsLiked) {
          await likeAPI.create(1, postId);
      } else {
          await likeAPI.delete(1, postId);
      }
    } catch {
      setPosts(prevPosts => prevPosts.map(post => {
          if (post.id === postId) {
              return {
                  ...post,
                  isLiked: currentIsLiked,
                  likes: currentIsLiked ? post.likes + 1 : post.likes - 1
              };
          }
          return post;
      }));
    }
  };
  const handlePostDelete = async (postId) => {
    const previousPosts = posts;
    setPosts(prevPosts => prevPosts.filter(post => post.id !== postId));
    try {
      await postAPI.delete(postId);
    } catch {
      setPosts(previousPosts);
      alert("Ошибка удаления поста, попробуйте позже.")
    }
  }

  useEffect(() => {
    let cancelled = false;
    async function load() {
      setIsLoading(true);
      setError(null);
      try {
        const data = await postAPI.getByAuthorId(id, page, pageSize);
        if (cancelled) return;
        setPosts(prev => page === 1 ? data.posts : [...prev, ...data.posts]);
        setHasMore(data.posts.length === pageSize);
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
  }, [hasMore, isLoading, isUserLoading]);

  useEffect(() => {
    const loadProfile = async () => {
      try {
        const userData = await userAPI.getById(id);
        setUser(userData);

        if (userData && userData.userId) {
          try {
            const followersData = await followAPI.getFollowers(userData.userId);
            setFollowers(followersData);
          } catch {
            setIsfollowError(true);
          } finally {
            setIsFollowersLoading(false);
          }

          try {
            const followingsData = await followAPI.getFollowings(userData.userId);
            setFollowings(followingsData);
          } catch {
            setIsfollowError(true);
          } finally {
            setIsFollowingsLoading(false);
          }
        } else {
          setIsFollowersLoading(false);
          setIsFollowingsLoading(false);
        }
      } catch { } finally {
        setIsUserLoading(false);
      }
    };
    loadProfile();
  }, [id]);

  if (isUserLoading) return <div className="container">Загрузка профиля</div>;
  if (!user) return <div className="container">Пользователь не найден</div>;

  return (
    <div className="profile-page">
      <header className="container profile-header">
        <div className="container profile-header-inner">
          <div className="profile-header-item">
            <h1 className="profile-username">{user.userName}</h1>
          </div>
          <div className="profile-header-item">
            {isFollowersLoading && isFollowingsLoading ? (
              <span>Загрузка</span>
            ) : followError ? (
              <span style={{ color: 'red' }}>Сервис подписок недоступен</span>
            ) : (
              <ul className="profile-header-list">
                <li className="profile-header-list-item">
                  <Link to={`/followers/${id}`} className="profile-header-list-link">
                    <span>{followers.totalCount}</span> подписчиков
                  </Link>
                </li>
                <li className="profile-header-list-item">
                  <Link to={`/followings/${id}`} className="profile-header-list-link">
                    <span>{followings.totalCount}</span> подписок
                  </Link>
                </li>
              </ul>
            )}
          </div>
        </div>
      </header>

      <main className="container feed-container">
        <h2 className="profile-posts-title">Публикации</h2>
        
        <ul className="feed">
          {posts.map(post => (
            <li key={post.id} className="feed-item">
              <PostCard 
                post={post} 
                onLikeToggle={handleLikeToggle} 
                onPostDelete={handlePostDelete}
              />
            </li>
          ))}
        </ul>

        <div ref={sentinelRef} className="feed-sentinel" />
          {isLoading && <div className="feed-loading">Загрузка...</div>}
          {error && <div className="feed-error">Ошибка: {error}</div>}
          {!hasMore && posts.length > 0 && (
              <div className="feed-end">Постов больше нет</div>
          )}
          {!isLoading && posts.length === 0 && (
          <div className="feed-empty">У пользователя пока нет публикаций</div>
          )}
      </main>
    </div>
  );
}
