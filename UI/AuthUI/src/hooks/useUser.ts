import { QueryClient, useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { AuthService } from "../services/AuthService";
import { LoginPayload, RegisterPayload } from "../types/Auth.types";

export const useUser = () => {
    return useQuery({
        queryKey: ["user"],
        queryFn: () => AuthService.fetchUser(),
        staleTime: Infinity,
    });
}

export const useLogin = () => {
    const queryClient = new QueryClient();
    return useMutation({
        mutationFn: (payload: LoginPayload) => AuthService.loginUser(payload),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["user"] });
        },
    })
}

export const useRegister = () => {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: (payload: RegisterPayload) => AuthService.registerUser(payload),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["user"] });
        },
    });
};

export const useLogout = () => {
    const queryClient = new QueryClient();
    return useMutation({
        mutationFn: () => AuthService.logoutUser(),
        onSuccess: () => {
            queryClient.removeQueries({ queryKey: ["user"] });
        },
    })
} 