'use client';

import { useAuth } from '@/contexts/AuthContext';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';

export default function Layout({ children }: { readonly children: React.ReactNode }) {
  const { isAuthenticated } = useAuth();
  const router = useRouter();
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!isAuthenticated) {
      const timeout = setTimeout(() => {
        setLoading(false);
      }, 1500);

      return () => clearTimeout(timeout);
    }

    router.push('/');
  }, [isAuthenticated, router]);

  if (loading) {
    return (
      <div className="flex h-screen flex-col items-center justify-center">
        <div className="border-foreground h-12 w-12 animate-spin rounded-full border-b-2" />
        <p className="text-foreground mt-4 text-sm">Estamos revisando si estas autenticado.</p>
      </div>
    );
  }

  return <>{children}</>;
}
