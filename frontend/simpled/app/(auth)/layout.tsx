'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/contexts/AuthContext';

export default function Layout({ children }: { children: React.ReactNode }) {
  const { isAuthenticated } = useAuth();
  const router = useRouter();
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (isAuthenticated) {
      const timeout = setTimeout(() => {
        router.push('/');
      }, 1500);

      return () => clearTimeout(timeout);
    }

    setLoading(false);
  }, [isAuthenticated, router]);

  if (loading) {
    return (
      <div className="flex h-screen flex-col items-center justify-center">
        <div className="border-foreground h-12 w-12 animate-spin rounded-full border-b-2" />
        <p className="text-foreground mt-4 text-sm">Ya estas autenticado no puedes acceder aquí.</p>
        <p className="text-foreground mt-4 text-sm">Redirigiendo a inicio...</p>
      </div>
    );
  }

  return <>{children}</>;
}
