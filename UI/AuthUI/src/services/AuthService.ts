import { LoginPayload, RegisterPayload } from "../types/Auth.types";
import { API } from "./api";
import { setAccessToken } from "../store/tokenStore";

export class AuthService {
  static registerUser = async (payload: RegisterPayload) => {
    const response = await API.post<Response>("/auth/register", payload);
    return response.data;
  };
  static loginUser = async (payload: LoginPayload) => {
    const response = await API.post<Response>("/auth/login", payload, {
      withCredentials: true,
    });
    return response.data;
  };
  static logoutUser = async (): Promise<void> => {
    await API.post("/auth/logout");
  };
  static fetchUser = async () => {
    const response = await API.get("/user/getDetails");
    return response.data;
  };
  static fetchNewAccessToken = async () => {
    const response = await API.get("/auth/refresh-token", {
      withCredentials: true,
    });
    const { accessToken, expiresAt } = response.data;

    setAccessToken(accessToken, expiresAt);
    return accessToken;
  };
}


git config --global user.name "Vaka Harsha Vardhan Reddy"
git config --global user.email "harshavaka44@gmail.com"
