import { useForm } from "react-hook-form";

type RegisterFormInputs = {
    email: string;
    fullName: string;
    password: string;
    confirmPassword: string;
}

type Props = {
    onGoogleSignIn?: () => void; // callback for Google sign-in
};
export function Register({ onGoogleSignIn }: Props) {
    const {
        register,
        handleSubmit,
        watch,
        formState: { errors,isSubmitting }
    } = useForm<RegisterFormInputs>();

    const handleRegister = (formData: RegisterFormInputs) => {
        console.log(formData);
    }
    return (
        <div className="space-y-4">
            <form onSubmit={handleSubmit(handleRegister)} className="space-y-4">
                <div>
                    <label className="block text-sm font-medium text-gray-700">Email</label>
                    <input type="email"
                        {...register('email', { required: 'Email is required' })}
                        className="mt-1 block w-full border border-gray-300 rounded-md p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"

                    />
                    {
                        errors.email &&
                        <p className="text-red-500 text-sm mt-1">{errors.email.message}</p>
                    }
                </div>
                <div>
                    <label className="block text-sm font-medium text-gray-700">Name</label>
                    <input type="text"
                        {...register('fullName', { required: 'Name is required' })}
                        className="mt-1 block w-full border border-gray-300 rounded-md p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"

                    />
                    {
                        errors.fullName &&
                        <p className="text-red-500 text-sm mt-1">{errors.fullName.message}</p>
                    }
                </div>
                <div>
                    <label className="block text-sm font-medium text-gray-700">Password</label>
                    <input type="password" {...register('password', {
                        required: 'Password is required',
                        minLength: { value: 6, message: 'Password should have min 6 characters' }
                    })}
                        className="mt-1 block w-full border border-gray-300 rounded-md p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"

                    />
                    {
                        errors.password &&
                        <p className="text-red-500 text-sm mt-1">{errors.password.message}</p>
                    }
                </div>
                <div>
                    <label className="block text-sm font-medium text-gray-700">Confirm Password</label>
                    <input type="password" {...register('confirmPassword', {
                        required: "Please confirm your password",
                        validate: (value) =>
                            value === watch("password") || "Passwords do not match",
                    })}
                        className="mt-1 block w-full border border-gray-300 rounded-md p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"

                    />
                    {
                        errors.confirmPassword &&
                        <p className="text-red-500 text-sm mt-1">{errors.confirmPassword.message}</p>
                    }
                </div>
                 <button
                    type="submit"
                    disabled={isSubmitting}
                    className="w-full bg-blue-500 hover:bg-blue-600 text-white font-semibold py-2 px-4 rounded-md"
                >
                    {isSubmitting ? "Registering..." : "Register"}
                </button>
            </form>
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
    )
}