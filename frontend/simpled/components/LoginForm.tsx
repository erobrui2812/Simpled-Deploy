'use client';

import type React from 'react';

import GoogleLoginButton from '@/components/GoogleLoginButton';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Checkbox } from '@/components/ui/checkbox';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { useAuth } from '@/contexts/AuthContext';
import { cn } from '@/lib/utils';
import { AtSign, KeyRound } from 'lucide-react';
import { useState } from 'react';

export function LoginForm({ className, ...props }: React.ComponentProps<'div'>) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [keepUserLoggedIn, setKeepUserLoggedIn] = useState(false);
  const { loginUser, loginWithGoogle } = useAuth();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await loginUser(email, password, keepUserLoggedIn);
    } catch (error) {
      console.error('Error al iniciar sesión:', error);
    }
  };

  return (
    <div className={cn('flex flex-col gap-6', className)} {...props}>
      <Card>
        <CardHeader>
          <CardTitle>Inicia sesión</CardTitle>
          <CardDescription>Escribe tu correo electrónico para iniciar sesión</CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit}>
            <div className="flex flex-col gap-6">
              <div className="grid gap-3">
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
              <div className="grid gap-3">
                <div className="flex items-center">
                  <Label htmlFor="password">Contraseña</Label>
                </div>
                <div className="relative flex items-center">
                  <KeyRound className="text-muted-foreground absolute left-3 h-5 w-5" />
                  <Input
                    className="pl-10"
                    id="password"
                    type="password"
                    value={password}
                    placeholder="••••••••"
                    onChange={(e) => setPassword(e.target.value)}
                    required
                  />
                </div>
              </div>
              <div className="flex items-center gap-3">
                <Checkbox
                  id="keepUserLoggedIn"
                  checked={keepUserLoggedIn}
                  onCheckedChange={(checked: boolean) => setKeepUserLoggedIn(checked)}
                />
                <Label htmlFor="keepUserLoggedIn">Mantener sesión iniciada</Label>
              </div>
              <div className="flex flex-col gap-3">
                <Button type="submit" className="w-full">
                  Iniciar sesión
                </Button>
              </div>
            </div>
          </form>
          <div className="mt-4 text-center text-sm">
            ¿No tienes cuenta?{' '}
            <a href="/registro" className="underline underline-offset-4">
              Regístrate ya
            </a>
          </div>
          <div className="mt-6 flex flex-col gap-3">
            <div className="relative">
              <div className="absolute inset-0 flex items-center">
                <span className="w-full border-t"></span>
              </div>
              <div className="relative flex justify-center text-xs uppercase">
                <span className="bg-background text-muted-foreground px-2">O continúa con</span>
              </div>
            </div>
            <div className="flex justify-center">
              <GoogleLoginButton text="signin_with" />
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
