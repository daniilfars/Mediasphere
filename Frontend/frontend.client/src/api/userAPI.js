import { request } from "./client";

export const userAPI = {
    getById: (userId) => 
        request(`/User/${userId}`),
    // Это будет вебхуком скорее всего от keycloak, поэтому не надо
    /*create: () => 
        request('/User'),*/
};