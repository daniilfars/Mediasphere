import { useEffect, useState } from "react";
import { useLocation, useNavigate, useParams } from "react-router-dom";
import { postAPI } from "../api/postAPI";
import { useForm } from "react-hook-form";

export default function EditPost() {
    const { register, handleSubmit, reset, formState } = useForm();

    const { id } = useParams();

    const location = useLocation();

    const [post, setPost] = useState(location.state?.post || null);
    const [loading, setLoading] = useState(!location.state?.post);
    const [error, setError] = useState(null);

    const navigate = useNavigate();

    useEffect(() => {
        if (post) return;

        const loadPost = async () => {
            try {
                const data = await postAPI.getById(id);
                setPost(data);
            } catch(e) {
                setError(e.message);
            } finally {
                setLoading(false);
            }
        }

        loadPost();
    }, [id, post]);

    useEffect(() => {
        if (!post) return;
        reset({ content: post.content ?? "" });
    }, [post, reset]);

    const onSubmit = async (data) => {
        try {
            const updated = await postAPI.update(id, data.content);
            setPost(updated);

            navigate(location.pathname, {
                replace: true,
                state: { post: updated },
            });

            reset({ content: updated.content });
            
            alert("Сохранено");
        } catch(e) {
            alert(e.message);
        }
    }

    if(loading) return <div className="container">Загрузка поста</div>
    if (error) return <div className="container">{error}</div>;
    if(!post) return <div className="container">Пост не найден</div>

    return (
        <div className="container">
            <form onSubmit={handleSubmit(onSubmit)}>
                <input {...register("content")}/>
                <button type="submit" disabled={!formState.isDirty}>Сохранить</button>
            </form>
        </div>
    );
}