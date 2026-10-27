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
    const [previewImageUrl, setPreviewImageUrl] = useState(null);
    const [selectedFile, setSelectedFile] = useState(null);

    const navigate = useNavigate();

    const handleFileChange = (e) => {
        const file = e.target.files?.[0];
        if (file) {
            setSelectedFile(file);
            setPreviewImageUrl(URL.createObjectURL(file));
        }
    };

    useEffect(() => {
        if (post) {
            setPreviewImageUrl(post.imageUrl || null);
        }
    }, [post]);

    useEffect(() => {
        if (post) {
            reset({ content: post.content ?? ""});
            return;
        }

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
    }, [id, reset]);

    const onSubmit = async (data) => {
        try {
            let updated = { ...post };

            if(formState.dirtyFields.content) {
                const res = await postAPI.update(id, data.content);
                if (res)
                    updated = res;
            }

            if(selectedFile) {
                const res = await postAPI.uploadImage(id, selectedFile);
                if (res)
                    updated = res;
            }

            setPost(updated);
            setSelectedFile(null);

            navigate(location.pathname, {
                replace: true,
                state: { post: updated },
            });

            reset({ content: updated.content });
            alert("Сохранено");
        } catch {
            alert("Не удалось сохранить");
        }
    }

    if(loading) return <div className="container">Загрузка поста</div>
    if (error) return <div className="container">{error}</div>;
    if(!post) return <div className="container">Пост не найден</div>

    return (
        <div className="container">
            <form onSubmit={handleSubmit(onSubmit)}>
                <div>
                    <input {...register("content")}/>
                    {previewImageUrl && <img src={previewImageUrl} alt="Картинка поста" />}
                    <input type="file" accept="image/jpeg, image/png, image/webp" onChange={handleFileChange}/>
                    <button type="submit" disabled={!formState.isDirty && !selectedFile}>Сохранить</button>
                </div>
            </form>
        </div>
    );
}