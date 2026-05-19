import axios from "axios";

const BASE_URL = import.meta.env.VITE_API_BASE_URL;

const api = axios.create({
    baseURL: BASE_URL,
    headers: {
        "Content-Type": "application/json",
    },
});

// Dodaj token ako postoji
api.interceptors.request.use((config) => {
    const token = localStorage.getItem("token");
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

// Globalni error handler
api.interceptors.response.use(
    (response) => response,
    (error) => {
        let message = "Request failed";
        if (error.response) {
            message = error.response.data?.message || `Error ${error.response.status}`;
        }
        return Promise.reject(new Error(message));
    }
);

export const http = {
    get: (path) => api.get(path).then((res) => res.data),
    post: (path, body) => api.post(path, body).then((res) => res.data),
    put: (path, body) => api.put(path, body).then((res) => res.data),
    delete: (path) => api.delete(path).then((res) => res.data),
};
