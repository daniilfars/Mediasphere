import { request } from "./client";

export const commentAPI = {
    get: (postId, page = 1, pageSize = 10) =>
        request(`/Comments?postId=${postId}&page=${page}&pageSize=${pageSize}`),

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