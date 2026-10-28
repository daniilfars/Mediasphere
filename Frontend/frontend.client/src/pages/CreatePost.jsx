import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { postAPI } from "../api/postAPI";
import { useForm } from "react-hook-form";
//import "./EditPost.css";

export default function CreatePost() {
    const { register, handleSubmit, reset, formState } = useForm();
    const navigate = useNavigate();

    const [selectedFile, setSelectedFile] = useState(null);
    const [previewUrl, setPreviewUrl] = useState("");

    const handleFileChange = (e) => {
        const file = e.target.files?.[0];
        if (!file) {
            return;
        }
        if (file.size > 5 * 1024 * 1024) {
            alert("Размер файла не должен превышать 5 мб");
            e.target.value = "";
            return;
        }

        setSelectedFile(file);
        setPreviewUrl(URL.createObjectURL(file));
    };

    useEffect(() => {
        return () => {
            if (previewUrl) URL.revokeObjectURL(previewUrl);
        };
    }, [previewUrl]);

    const onSubmit = async (data) => {
        try {
            const createdPost = await postAPI.create(data.content, selectedFile);

            reset({ content: createdPost.content });
            alert("Пост успешно создан");
            navigate(-1);
        } catch {
            alert("Не удалось сохранить");
        }
    }

    return (
        <div className="container editpost-container">
            <form className="editpost-form" onSubmit={handleSubmit(onSubmit)}>
                <input {...register("content", { required: "Текст поста не заполнен" })} placeholder="Введите текст поста"/>
                {previewUrl && <img src={previewUrl} alt="Картинка поста" />}
                <input type="file" accept="image/jpeg, image/png, image/webp" onChange={handleFileChange}/>
                <button type="submit" disabled={!formState.isDirty}>Сохранить</button>
            </form>
        </div>
    );
}