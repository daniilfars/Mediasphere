import { request } from "./client";

export const likeAPI = {
    get: (targetType, contentId) =>
        request(`/Like?targetType=${targetType}&contentId=${contentId}`),

    create: (targetType, contentId) =>
        request('/Like', {
            method: 'POST',
            body: JSON.stringify({targetType, contentId})
        }),

    delete: (id) =>
        request(`/Like/${id}`, {
            method: 'DELETE',
        }),
};