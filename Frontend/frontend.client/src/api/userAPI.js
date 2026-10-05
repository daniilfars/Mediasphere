import { request } from "./client";

export const userAPI = {
    getById: (userId) => 
        request(`/User/${userId}`),
};