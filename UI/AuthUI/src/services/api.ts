import axios, { InternalAxiosRequestConfig } from "axios";
import { getAccessToken, isTokenExpired } from "../store/tokenStore";
import { getRefreshPromise } from "../store/tokenStore";
import { setRefreshPromise } from "../store/tokenStore";
import { AuthService } from "./AuthService";

const IGNORED_ROUTES = ["/auth/login", "/auth/register"];

export const API = axios.create({
  baseURL: "https://localhost:5001/api/",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
  },
});

API.interceptors.request.use(
  async (
    config: InternalAxiosRequestConfig
  ): Promise<InternalAxiosRequestConfig> => {
    if (
      config.url?.includes("/auth/login") ||
      config.url?.includes("/auth/register") ||
      config.url?.includes("/auth/refresh-token")
    ) {
      return config;
    }

    let token = getAccessToken();
    if (!isTokenExpired() && token) {
      if (config.headers) {
        config.headers["Authorization"] = `Bearer ${token}`;
      } else {
        config.headers = { Authorization: `Bearer ${token}` } as any;
      }
      return config;
    }

    // Token expired or missing → refresh
    if (!getRefreshPromise()) {
      setRefreshPromise(
        AuthService.fetchNewAccessToken()
          .catch((err) => {
            console.error("Token refresh failed:", err);
            throw err;
          })
          .finally(() => setRefreshPromise(null))
      );
    }

    try {
      token = await getRefreshPromise();
      if (!token) throw new Error("No token returned from refresh");

      if (config.headers) {
        config.headers["Authorization"] = `Bearer ${token}`;
      } else {
        config.headers = { Authorization: `Bearer ${token}` } as any;
      }
      return config;
    } catch (err) {
      window.location.href = "/auth/login";
      throw err;
    }
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
