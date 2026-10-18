import { useState, useEffect } from "react";
import { likeAPI } from '@/api/likeAPI';
import { postAPI } from '@/api/postAPI';

const PAGE_SIZE = 12;

export function usePostFeed(fetchPostsFn) {
  const [posts, setPosts] = useState([]);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    setPosts([]);
    setPage(1);
    setHasMore(true);
    setError(null);
  }, [fetchPostsFn]);

  useEffect(() => {
    let cancelled = false;
    
    async function load() {
      setIsLoading(true);
      setError(null);
      try {
        const data = await fetchPostsFn(page, PAGE_SIZE);
        if (cancelled) return;

        setPosts(prev => page === 1 ? data.posts : [...prev, ...data.posts]);
        setHasMore(data.posts.length === PAGE_SIZE);
      } catch (err) {
        if (cancelled) return;
        setError(err.message || 'Ошибка загрузки');
      } finally {
        if (!cancelled) setIsLoading(false);
      }
    }

    load();
    return () => { cancelled = true; };
  }, [page, fetchPostsFn]);

  const handleLikeToggle = async (postId, currentIsLiked) => {
    setPosts(prevPosts => prevPosts.map(post => {
      if (post.id === postId) {
        return { ...post, isLiked: !currentIsLiked, likes: currentIsLiked ? post.likes - 1 : post.likes + 1 };
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
          return { ...post, isLiked: currentIsLiked, likes: currentIsLiked ? post.likes + 1 : post.likes - 1 };
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
      alert("Ошибка удаления поста, попробуйте позже.");
    }
  };

  const loadNextPage = () => {
    if (!isLoading && hasMore) {
      setPage(p => p + 1);
    }
  };

  return { posts, isLoading, error, hasMore, handleLikeToggle, handlePostDelete, loadNextPage };
}