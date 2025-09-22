let accessToken:string|null = null;
let expiresAt:Date|null = null;
let refreshPromise:Promise<string> |null = null;

export const getAccessToken = () => accessToken;

export const setAccessToken = (token:string, expiresAt:Date) => {
  accessToken = token;
  expiresAt = expiresAt;
};

export const isTokenExpired = () => {
  if (!accessToken || !expiresAt) return true;
  return Date.now() >= expiresAt.getTime();
};

export const clearToken = () => {
  accessToken = null;
  expiresAt = null;
  refreshPromise = null;
};

export const setRefreshPromise = (promise:Promise<string>|null) => {
  refreshPromise = promise;
};

export const getRefreshPromise = () => refreshPromise;
