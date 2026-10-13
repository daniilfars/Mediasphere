import { useEffect, useRef, useState } from "react";
import { postAPI } from '@/api/postAPI';
import { likeAPI } from '@/api/likeAPI';
import PostCard from "@/components/post/PostCard";
import './Feed.css';

export default function Feed() {
  const [posts, setPosts] = useState([]);
  const [error, setError] = useState(null);
  
  const pageSize = 12;
  const sentinelRef = useRef(null);

  const apiState = useRef({
    page: 1,
    isLoading: false,
    hasMore: true,
  });

  const [isLoadingUI, setIsLoadingUI] = useState(false);
  const [hasMoreUI, setHasMoreUI] = useState(true);

  const loadData = async (targetPage) => {
    if (apiState.current.isLoading || !apiState.current.hasMore) return;

    apiState.current.isLoading = true;
    setIsLoadingUI(true);
    setError(null);

    try {
      const data = await postAPI.getAll(targetPage, pageSize);
      
      setPosts(prev => targetPage === 1 ? data.posts : [...prev, ...data.posts]);
      
      const isMoreAvailable = data.posts.length === pageSize;
      apiState.current.hasMore = isMoreAvailable;
      setHasMoreUI(isMoreAvailable);
      
      apiState.current.page = targetPage;
    } catch (err) {
      setError(err.message || 'Ошибка загрузки');
    } finally {
      apiState.current.isLoading = false;
      setIsLoadingUI(false);
    }
  };

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
    let deletedPost = null;

    setPosts(prevPosts => {
      deletedPost = prevPosts.find(p => p.id === postId);
      return prevPosts.filter(post => post.id !== postId);
    });

    try {
      await postAPI.delete(postId);
    } catch {
      if (deletedPost) {
        setPosts(prevPosts => [...prevPosts, deletedPost]);
      }
      alert("Ошибка удаления поста, попробуйте позже.");
    }
  };

  useEffect(() => {
    loadData(1);
  }, []);

  useEffect(() => {
    const sentinel = sentinelRef.current;
    if (!sentinel) return;

    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting && !apiState.current.isLoading && apiState.current.hasMore) {
          const nextPage = apiState.current.page + 1;
          loadData(nextPage);
        }
      },
      { rootMargin: '200px' }
    );

    observer.observe(sentinel);
    return () => observer.disconnect();
  }, []);

  return (
    <div className="container feed-container">
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
      
      {isLoadingUI && <div className="feed-loading">Загрузка...</div>}
      {error && <div className="feed-error">Ошибка: {error}</div>}
      {!hasMoreUI && posts.length > 0 && (
        <div className="feed-end">Постов больше нет</div>
      )}
    </div>
  );
}
