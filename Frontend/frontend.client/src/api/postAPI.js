import { request } from "./client";

export const postAPI = {
    getAll: (page = 1, pageSize = 10) =>
        request(`/Post?page=${page}&pageSize=${pageSize}`),

    getById: (id) => 
        request(`/Post/${id}`),

    create: (content, file) => {
        const formData = new FormData();
        formData.append('content', content)
        formData.append('file', file);
        return request('/Post', {
            method: 'POST',
            body: formData,
        });
    },

    update: (id, content) =>
        request('/Post', {
            method: 'PUT',
            body: JSON.stringify({id, content}),
        }),

    delete: (id) =>
        request(`/Post/${id}`, { method: 'DELETE' }),

    uploadImage: (id, file) => {
        const formData = new FormData();
        formData.append('file', file);
        return request(`/Post/${id}/upload-image`, {
            method: 'POST',
            body: formData,
        });
    },
};