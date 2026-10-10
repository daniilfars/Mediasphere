import { useEffect, useRef, useState } from "react";
import { postAPI } from '@/api/postAPI';
import PostCard from "@/components/post/PostCard";
import './Feed.css';

export default function Feed() {
    const [posts, setPosts] = useState([]);
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
                const data = await postAPI.getAll(page, pageSize);
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
    }, [page]);

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

    return (
        <div className="container feed-container">
            <ul className="feed">
                {posts.map(post => (
                    <li key={post.id} className="feed-item">
                        <PostCard post={post} />
                    </li>
                ))}
            </ul>

            <div ref={sentinelRef} className="feed-sentinel" />

            {isLoading && <div className="feed-loading">Загрузка...</div>}
            {error && <div className="feed-error">Ошибка: {error}</div>}
            {!hasMore && posts.length > 0 && (
                <div className="feed-end">Постов больше нет</div>
            )}
        </div>
    );
}