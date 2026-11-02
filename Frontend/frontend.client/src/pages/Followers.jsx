import { followAPI } from "../api/followAPI";
import { useEffect, useRef, useState } from "react";
import { Link, useParams } from "react-router-dom";
import "./Follow.css";


export default function Followers() {
    const { userName, id } = useParams();

    const [followers, setFollowers] = useState([]);
    const [totalCount, setTotalCount] = useState(0);
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
                const data = await followAPI.getFollowers(id, page, pageSize);
                if (cancelled) return;

                setFollowers(prev => page === 1 ? data.followers : [...prev, ...data.followers]);
                setTotalCount(data.totalCount);
                setHasMore(data.followers.length === pageSize);
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
        <div className="container follow-container">
            <Link to={`/profile/${id}`} className="follow-name">{userName}</Link>
            <p className="follow-count"><span>{totalCount}</span> подписчиков</p>
            <ul className="follow-list">
                {followers.map(follower => (
                <li key={follower.followerId}>
                    <Link to={`/profile/${follower.followerId}`}>
                    {follower.followerUserName}
                    </Link>
                </li>
                ))}
            </ul>

            <div ref={sentinelRef} className="feed-sentinel" />

            {isLoading && <div className="follow-loading">Загрузка...</div>}
            {error && <div className="follow-error">Ошибка: {error}</div>}
        </div>
    );
}