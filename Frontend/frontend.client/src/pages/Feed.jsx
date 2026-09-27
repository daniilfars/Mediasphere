import { useEffect, useState } from "react";
import { postAPI } from '@/api/postAPI';
import PostCard from "@/components/post/PostCard";
import './Feed.css';

export default function Feed() {
  const [posts, setPosts] = useState([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [isLoading, setIsLoading] = useState(true);
  const pageSize = 12;

  const totalPages = Math.ceil(totalCount / pageSize)

  const loadPosts = async (currentPage = page) => {
    try {
        setIsLoading(true);
        const data = await postAPI.getAll(currentPage, pageSize);
        setPosts(data.posts);
        setTotalCount(data.totalCount);
    } catch { }
    finally {
        setIsLoading(false);
    }
  };

  useEffect(() => {
    loadPosts();
  }, [page]);

  if(isLoading) {
        return <div className="feed-loading">Загрузка...</div>
    }

  return (
    <div className="container feed-container">
        <ul className="feed">
            {posts.map(post => (
                <li key={post.id} className="feed-item">
                    <PostCard post={post}/>
                </li>
            ))}
        </ul>
    </div>
  );
}