import { Link, useParams } from 'react-router-dom';
import { userAPI } from '../api/userAPI';
import { followAPI } from '../api/followAPI';
import { postAPI } from '../api/postAPI';
import { useState, useEffect, useRef, useCallback } from 'react';
import { usePostFeed } from "@/hooks/usePostFeed";
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

  const fetchUserPosts = useCallback((page, pageSize) => {
    return postAPI.getByAuthorId(id, page, pageSize); 
  }, [id]);

  const {posts, isLoading: isPostsLoading, error: postsError, hasMore, handleLikeToggle, handlePostDelete, loadNextPage} = usePostFeed(fetchUserPosts);

  const sentinelRef = useRef(null);

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

  useEffect(() => {
    const sentinel = sentinelRef.current;
    if (!sentinel || !hasMore || isPostsLoading) return;

    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting) {
          loadNextPage();
        }
      },
      { rootMargin: '200px' }
    );

    observer.observe(sentinel);
    return () => observer.disconnect();
  }, [hasMore, isPostsLoading, loadNextPage]);

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

        {isPostsLoading && <div className="feed-loading">Загрузка постов...</div>}
        {postsError && <div className="feed-error">Ошибка: {postsError}</div>}
        {!hasMore && posts.length > 0 && (
          <div className="feed-end">Постов больше нет</div>
        )}
        {!isPostsLoading && posts.length === 0 && (
          <div className="feed-empty">У пользователя пока нет публикаций</div>
        )}
      </main>
    </div>
  );
}
