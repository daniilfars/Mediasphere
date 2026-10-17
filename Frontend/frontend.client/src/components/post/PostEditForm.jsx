import { useEffect, useRef, useState } from 'react';
import './PostEditForm.css';

export default function PostEditForm({ post, onSave, onImageUpdate, onCancel }) {
    const [content, setContent] = useState(post.content ?? '');
    const [imageUrl, setImageUrl] = useState(post.imageUrl ?? null);
    const [isSaving, setIsSaving] = useState(false);
    const [isUploadingImage, setIsUploadingImage] = useState(false);
    const fileInputRef = useRef(null);
    const textareaRef = useRef(null);

    useEffect(() => {
        if (textareaRef.current) {
            textareaRef.current.focus();
            textareaRef.current.setSelectionRange(textareaRef.current.value.length, textareaRef.current.value.length);
        }
    }, []);

    const handleImageClick = () => fileInputRef.current?.click();

    const handleImageChange = async (e) => {
        const file = e.target.files?.[0];
        if (!file) return;
        e.target.value = '';

        if (!file.type.startsWith('image/')) { alert('Можно загрузить только изображение'); return; }
        if (file.size > 5 * 1024 * 1024) { alert('Максимум 5 МБ'); return; }

        setIsUploadingImage(true);
        try {
            const updated = await onImageUpdate(post.id, file);
            setImageUrl(updated.imageUrl);
        } catch (err) {
            alert(err.message || 'Не удалось загрузить картинку');
        } finally {
            setIsUploadingImage(false);
        }
    };

    const handleSave = async () => {
        const trimmed = content.trim();
        if (!trimmed) return;

        setIsSaving(true);
        try {
            await onSave(post.id, trimmed);
            onCancel();
        } catch (err) {
            alert(err.message || 'Не удалось сохранить');
        } finally {
            setIsSaving(false);
        }
    };

    const handleKeyDown = (e) => {
        if (e.key === 'Escape') onCancel();
        if (e.key === 'Enter' && (e.ctrlKey || e.metaKey)) handleSave();
    };

    return (
        <article className="post-card post-edit-card">
            <header className="post-card-header">
                <span className="post-edit-header-text">Редактирование</span>
            </header>

            <div className="post-card-body">
                <div className="post-edit-image">
                    {imageUrl ? <img src={imageUrl} alt="" /> : <div className="post-edit-image-placeholder">Картинки нет</div>}
                    <input ref={fileInputRef} type="file" accept="image/*" onChange={handleImageChange} style={{ display: 'none' }} />
                    <button type="button" className="post-edit-image-btn" onClick={handleImageClick} disabled={isUploadingImage || isSaving}>
                        {isUploadingImage ? 'Загрузка...' : imageUrl ? 'Заменить картинку' : 'Добавить картинку'}
                    </button>
                </div>

                <textarea ref={textareaRef} className="post-edit-textarea" value={content} onChange={(e) => setContent(e.target.value)} onKeyDown={handleKeyDown} placeholder="Текст поста..." disabled={isSaving} />
            </div>

            <footer className="post-card-actions post-edit-actions">
                <button type="button" className="post-edit-btn post-edit-save" onClick={handleSave} disabled={isSaving || !content.trim()}>
                    {isSaving ? 'Сохранение...' : 'Сохранить'}
                </button>
                <button type="button" className="post-edit-btn post-edit-cancel" onClick={onCancel} disabled={isSaving}>Отмена</button>
            </footer>
        </article>
    );
}