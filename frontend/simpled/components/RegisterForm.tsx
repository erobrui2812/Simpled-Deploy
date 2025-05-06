'use client';

import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { useAuth } from '@/contexts/AuthContext';
import { cn } from '@/lib/utils';
import { AtSign, Github, ImageIcon, KeyRound, Mail } from 'lucide-react';
import React, { useState } from 'react';
import { toast } from 'react-toastify';

export function RegisterForm({ className, ...props }: React.ComponentProps<'div'>) {
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [image, setImage] = useState<File | null>(null);
  const { registerUser, externalLogin } = useAuth();

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (!e.target.files) return;
    const file = e.target.files[0];
    if (file.size > 2 * 1024 * 1024) {
      toast.error('El archivo es demasiado grande (máximo 2MB)');
      return;
    }
    setImage(file);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await registerUser(name, email, password, image);
    } catch {
      toast.error('Error al registrarse. Intenta nuevamente.');
    }
  };

  return (
    <div className={cn('flex flex-col gap-6', className)} {...props}>
      <Card>
        <CardHeader>
          <CardTitle>Regístrate</CardTitle>
          <CardDescription>Escribe tus datos para crear una cuenta</CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit}>
            <div className="flex flex-col gap-6">
              <div className="grid gap-3">
                <Label htmlFor="name">Nombre completo</Label>
                <div className="relative flex items-center">
                  <AtSign className="text-muted-foreground absolute left-3 h-5 w-5" />
                  <Input
                    id="name"
                    type="text"
                    placeholder="Tu nombre completo"
                    className="pl-10"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    required
                  />
                </div>
              </div>

              <div className="grid gap-3">
                <Label htmlFor="email">Email</Label>
                <div className="relative flex items-center">
                  <AtSign className="text-muted-foreground absolute left-3 h-5 w-5" />
                  <Input
                    id="email"
                    type="email"
                    placeholder="email@dominio.com"
                    className="pl-10"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                  />
                </div>
              </div>

              <div className="grid gap-3">
                <Label htmlFor="password">Contraseña</Label>
                <div className="relative flex items-center">
                  <KeyRound className="text-muted-foreground absolute left-3 h-5 w-5" />
                  <Input
                    id="password"
                    type="password"
                    placeholder="••••••••"
                    className="pl-10"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                  />
                </div>
              </div>

              <div className="grid gap-3">
                <Label htmlFor="image">Foto de perfil (opcional)</Label>
                <div className="relative flex items-center">
                  <ImageIcon className="text-muted-foreground absolute left-3 h-5 w-5" />
                  <Input
                    id="image"
                    type="file"
                    accept="image/*"
                    className="pl-10"
                    onChange={handleImageChange}
                  />
                </div>
              </div>

              <div className="flex flex-col gap-3">
                <Button type="submit" className="w-full">
                  Registrarse
                </Button>

                <div className="relative my-2">
                  <div className="absolute inset-0 flex items-center">
                    <span className="w-full border-t"></span>
                  </div>
                  <div className="relative flex justify-center text-xs uppercase">
                    <span className="bg-background text-muted-foreground px-2">
                      O registrarse con
                    </span>
                  </div>
                </div>

                <div className="grid grid-cols-2 gap-3">
                  <Button
                    type="button"
                    variant="outline"
                    onClick={() => externalLogin('GitHub')}
                    className="w-full"
                  >
                    <Github className="mr-2 h-4 w-4" />
                    GitHub
                  </Button>
                  <Button
                    type="button"
                    variant="outline"
                    onClick={() => externalLogin('Google')}
                    className="w-full"
                  >
                    <Mail className="mr-2 h-4 w-4" />
                    Google
                  </Button>
                </div>
              </div>
            </div>

            <div className="mt-4 text-center text-sm">
              ¿Ya tienes cuenta?{' '}
              <a href="/login" className="underline underline-offset-4">
                Inicia sesión
              </a>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
