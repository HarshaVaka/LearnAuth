import './App.css'
import { Navigate, Route, Routes } from 'react-router-dom'
import AuthPage from './Pages/AuthPage';
import Home from './Pages/Home';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Toaster } from 'react-hot-toast';
import { GlobalLoader } from './Components/GlobalLoader';

function App() {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: {
        refetchOnWindowFocus: false,
        retry: 1,
      },
    },
  });
  return (
    <>
      <QueryClientProvider client={queryClient}>
        <GlobalLoader/>
         <Toaster position="top-right" reverseOrder={false} />
        <Routes>
          {/* Default redirect to /home */}
          <Route path="/" element={<Navigate to="/home" replace />} />
          <Route path="/home" element={<Home />} />
          <Route path="/auth/*" element={<AuthPage />} />
          <Route path="*" element={<div className="p-6 text-center">404 - Page Not Found</div>} />
        </Routes>
      </QueryClientProvider>
    </>
  )
}

export default App
