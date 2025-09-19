import axios from "axios";

const IGNORED_ROUTES = ["/auth/login", "/auth/register"];

export const API = axios.create({
    baseURL: "https://localhost:44328/api/",
    withCredentials: true,
    headers: {
        "Content-Type": "application/json",
    },
});

API.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem("token"); // for JWT fallback
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

API.interceptors.response.use(
  (response) => response,
  (error) => {
    const url = error.config?.url;

    // If the request URL is in the ignored list, skip redirect
    if (!IGNORED_ROUTES.includes(url || "") && error.response?.status === 401) {
      console.error("Session expired. Redirecting to login...");
      window.location.href = "/auth/login";
    }
    return Promise.reject(error);
  }
);