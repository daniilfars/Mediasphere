import { request } from "./client";

export const commentAPI = {
    getAll: (postId, page = 1, pageSize = 10) =>
        request(`/Comment?PostId=${postId}&Page=${page}&PageSize=${pageSize}`),

    create: (postId, content) =>
        request('/Comment', {
            method: 'POST',
            body: JSON.stringify({postId, content})
        }),

    update: (id, content) =>
        request('/Comment', {
            method: 'PUT',
            body: JSON.stringify({id, content})
        }),

    delete: (id) =>
        request(`/Comment/${id}`, {
            method: 'DELETE',
        }),
};