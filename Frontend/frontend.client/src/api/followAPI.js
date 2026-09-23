import { request } from "./client";

export const followAPI = {
    getFollowers: (userId, page = 1, pageSize = 10) =>
        request(`/Follow/followers?UserId=${userId}&Page=${page}&PageSize=${pageSize}`),

    getFollowings: (userId, page = 1, pageSize = 10) =>
        request(`/Follow/followings?UserId=${userId}&Page=${page}&PageSize=${pageSize}`),

    create: (followingId) =>
        request(`/Follow/${followingId}`, {
            method: 'POST',
        }),

    delete: (followerId, followingId) =>
        request('/Follow', {
            method: 'DELETE',
            body: JSON.stringify({followerId, followingId})
        }),
};