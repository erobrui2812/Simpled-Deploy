'use client';

import { cn } from '@/lib/utils';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { useState } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { AtSign, KeyRound } from 'lucide-react';

export function RegisterForm({ className, ...props }: React.ComponentProps<'div'>) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const { registerUser } = useAuth();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await registerUser(email, password);
    } catch (error) {
      console.error('Error al iniciar sesión:', error);
    }
  };

  return (
    <div className={cn('flex flex-col gap-6', className)} {...props}>
      <Card>
        <CardHeader>
          <CardTitle>Regístrate</CardTitle>
          <CardDescription>
            Escribe tu correo electrónico y contraseña para registrarte
          </CardDescription>
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
              <div className="flex flex-col gap-3">
                <Button type="submit" className="w-full">
                  Registrarse
                </Button>
              </div>
            </div>
            <div className="mt-4 text-center text-sm">
              ¿Ya tienes una cuenta? Dirigete a{' '}
              <a href="/login" className="underline underline-offset-4">
                Login
              </a>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
