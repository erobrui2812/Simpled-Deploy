'use client';

import { useAuth } from '@/contexts/AuthContext';
import { useRouter, useSearchParams } from 'next/navigation';
import { useEffect, useState } from 'react';
import { toast } from 'react-toastify';

export default function ExternalLoginCallback() {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const searchParams = useSearchParams();
  const router = useRouter();
  const { auth } = useAuth();

  useEffect(() => {
    const handleCallback = async () => {
      try {
        setLoading(true);
        const provider = searchParams.get('provider');

        if (!provider) {
          setError('Proveedor no especificado');
          return;
        }

        // The backend will handle the authentication and redirect back with token and userId
        // We just need to check if we have a token in the URL
        const token = searchParams.get('token');
        const userId = searchParams.get('id');

        if (token && userId) {
          // Store the token and userId
          localStorage.setItem('token', token);
          localStorage.setItem('userId', userId);

          toast.success('Inicio de sesión exitoso');
          router.push('/');
        } else {
          // If we don't have a token, it means the backend is still processing
          // We'll show a loading state
          setLoading(true);
        }
      } catch (error) {
        console.error('Error en el callback de autenticación externa:', error);
        setError('Error al procesar la autenticación externa');
      } finally {
        setLoading(false);
      }
    };

    // If we're already authenticated, redirect to home
    if (auth.token) {
      router.push('/');
      return;
    }

    handleCallback();
  }, [searchParams, router, auth.token]);

  if (loading) {
    return (
      <div className="flex h-screen flex-col items-center justify-center">
        <div className="border-foreground h-12 w-12 animate-spin rounded-full border-b-2" />
        <p className="text-foreground mt-4 text-sm">Procesando autenticación...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex h-screen flex-col items-center justify-center">
        <div className="rounded-full bg-red-100 p-4 text-red-600 dark:bg-red-900/20 dark:text-red-400">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            className="h-10 w-10"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
        </div>
        <h1 className="mt-4 text-xl font-bold">Error de autenticación</h1>
        <p className="text-muted-foreground mt-2">{error}</p>
        <button
          onClick={() => router.push('/login')}
          className="mt-6 rounded bg-blue-600 px-4 py-2 text-white hover:bg-blue-700"
        >
          Volver al inicio de sesión
        </button>
      </div>
    );
  }

  return (
    <div className="flex h-screen flex-col items-center justify-center">
      <div className="border-foreground h-12 w-12 animate-spin rounded-full border-b-2" />
      <p className="text-foreground mt-4 text-sm">Redirigiendo...</p>
    </div>
  );
}
