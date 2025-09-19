import { LoginPayload, RegisterPayload } from "../types/Auth.types";
import { API } from "./api";

export class AuthService {
    static registerUser = async (payload: RegisterPayload) => {
        const response = await API.post<Response>("/auth/register", payload);
        return response.data;
    }
    static loginUser = async (payload: LoginPayload) => {
        const response = await API.post<Response>("/auth/login", payload);
        return response.data;
    }
    static logoutUser = async (): Promise<void> => {
        await API.post("/auth/logout");
    }
    static fetchUser = async () => {
        const response = await API.get("/user/getDetails");
        return response.data;
    };
}