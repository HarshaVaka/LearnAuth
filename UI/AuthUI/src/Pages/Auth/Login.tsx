import { useForm } from "react-hook-form"
import { useLogin } from "../../hooks/useUser";
import { useEffect } from "react";
import toast from "react-hot-toast";

type LoginFormInputs = {
    email: string;
    password: string;
}

type Props = {
    onGoogleSignIn?: () => void; // callback for Google sign-in
};
export function Login({ onGoogleSignIn }: Props) {
    const { mutate: login, isError, error } = useLogin();
    const {
        register,
        handleSubmit,
        formState: { errors, isSubmitting }
    } = useForm<LoginFormInputs>();

    const handleLogin = (formData: LoginFormInputs) => {
        login(formData);
    }
    useEffect(()=>{
        if(isError){
            toast.error(error.message || "Login failed");
        }
    },[isError,error])
    return (
        <div className="space-y-4">
            <form onSubmit={handleSubmit(handleLogin)} className="space-y-4">
                <div>
                    <label htmlFor="email" className="block text-sm font-medium text-gray-700">Email</label>
                    <input
                        type="email"
                        {...register("email", { required: "Email is required" })}
                        className="mt-1 block w-full border border-gray-300 rounded-md p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                    {errors.email && (
                        <p className="text-red-500 text-sm mt-1">{errors.email.message}</p>
                    )}
                </div>

                <div>
                    <label htmlFor="password" className="block text-sm font-medium text-gray-700">Password</label>
                    <input
                        type="password"
                        {...register("password", { required: "Password is required" })}
                        className="mt-1 block w-full border border-gray-300 rounded-md p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                    {errors.password && (
                        <p className="text-red-500 text-sm mt-1">{errors.password.message}</p>
                    )}
                </div>

                {/* Submit Button */}
                <button
                    type="submit"
                    disabled={isSubmitting}
                    className="w-full bg-blue-500 hover:bg-blue-600 text-white font-semibold py-2 px-4 rounded-md"
                >
                    {isSubmitting ? "Logging in..." : "Login"}
                </button>
            </form>
            {/* Divider */}
            <div className="flex items-center my-4">
                <hr className="flex-1 border-gray-300" />
                <span className="mx-2 text-gray-400 text-sm">or</span>
                <hr className="flex-1 border-gray-300" />
            </div>

            {/* Google Sign-In Button */}
            <button
                type="button"
                onClick={onGoogleSignIn}
                className="w-full flex items-center justify-center border border-gray-300 rounded-md py-2 px-4 hover:bg-gray-100"
            >
                <img
                    src="https://fonts.gstatic.com/s/i/productlogos/googleg/v6/24px.svg" alt="Google Logo"
                    className="w-5 h-5 mr-2"
                />
                Sign in with Google
            </button>
        </div>
    );
}

