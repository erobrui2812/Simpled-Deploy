'use client';

import type React from 'react';

import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { ArrowRight, AtSign, KeyRound } from 'lucide-react';
import { useRouter, useSearchParams } from 'next/navigation';
import { useState } from 'react';
import { toast } from 'react-toastify';

const API_URL = 'http://localhost:5193/';

export default function VerifyCodePage() {
  const [code, setCode] = useState('');
  const [email, setEmail] = useState('');
  const [loading, setLoading] = useState(false);
  const router = useRouter();
  const searchParams = useSearchParams();

  // Get email from query params if available
  const emailFromQuery = searchParams.get('email');

  // Use email from query if available
  useState(() => {
    if (emailFromQuery) {
      setEmail(emailFromQuery);
    }
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!email || !code) {
      toast.error('Por favor, introduce tu email y el código de verificación');
      return;
    }

    setLoading(true);

    try {
      const response = await fetch(`${API_URL}api/Auth/confirm-code`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          email,
          code,
        }),
      });

      if (!response.ok) {
        throw new Error('Código de verificación inválido');
      }

      toast.success('Email verificado correctamente. Ahora puedes iniciar sesión.');
      router.push('/login');
    } catch (error) {
      console.error('Error al verificar el código:', error);
      toast.error('Código de verificación inválido. Por favor, inténtalo de nuevo.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex h-screen w-full items-center justify-center">
      <Card className="w-full max-w-md">
        <CardHeader>
          <CardTitle>Verificación de cuenta</CardTitle>
          <CardDescription>
            Introduce el código de verificación que hemos enviado a tu correo electrónico
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-6">
            <div className="space-y-3">
              <Label htmlFor="email">Email</Label>
              <div className="relative flex items-center">
                <AtSign className="text-muted-foreground absolute left-3 h-5 w-5" />
                <Input
                  className="pl-10"
                  id="email"
                  type="email"
                  placeholder="email@dominio.com"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                />
              </div>
            </div>

            <div className="space-y-3">
              <Label htmlFor="code">Código de verificación</Label>
              <div className="relative flex items-center">
                <KeyRound className="text-muted-foreground absolute left-3 h-5 w-5" />
                <Input
                  className="pl-10"
                  id="code"
                  type="text"
                  placeholder="Introduce el código de 6 dígitos"
                  value={code}
                  onChange={(e) => setCode(e.target.value)}
                  required
                />
              </div>
            </div>

            <Button type="submit" className="w-full" disabled={loading}>
              {loading ? 'Verificando...' : 'Verificar código'}
              {!loading && <ArrowRight className="ml-2 h-4 w-4" />}
            </Button>

            <div className="text-center text-sm">
              <p>
                ¿No has recibido el código?{' '}
                <a href="/login" className="text-primary hover:underline">
                  Volver al inicio de sesión
                </a>
              </p>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
