import { Route, Routes, useLocation, useNavigate } from "react-router-dom";
import { NavTabs, Tab } from "../Components/NavTab";
import { Login } from "./Auth/Login";
import { Register } from "./Auth/Register";

export default function AuthPage() {
    const navigate = useNavigate();
    const location = useLocation();

    const tabs: Tab[] = [
        { label: "Login", value: "/auth/login" },
        { label: "Register", value: "/auth/register" },
    ];

    const handleTabChange = (tabValue: string) => {
        navigate(tabValue);
    };

    const handleGoogleSignIn = () => {
        // Could open OAuth popup or call backend
        console.log("Google Sign-In triggered!");
    };
    return (
        <div className="max-w-md mx-auto mt-10 bg-white shadow rounded-lg">
            <NavTabs
                tabs={tabs}
                activeTab={location.pathname}  // current URL determines active
                onTabChange={handleTabChange}
            />

            <div className="p-6">
                <Routes>
                    <Route path="login" element={<Login onGoogleSignIn={handleGoogleSignIn} />} />
                    <Route path="register" element={<Register />} />
                </Routes>
            </div>
        </div>
    );
}