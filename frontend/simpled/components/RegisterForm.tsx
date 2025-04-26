'use client';

import type React from 'react';

import GoogleLoginButton from '@/components/GoogleLoginButton';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { useAuth } from '@/contexts/AuthContext';
import { cn } from '@/lib/utils';
import { AtSign, BadgeIcon as IdCard, ImageIcon as Image, KeyRound } from 'lucide-react';
import { useState } from 'react';
import { toast } from 'react-toastify';

export function RegisterForm({ className, ...props }: React.ComponentProps<'div'>) {
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [image, setImage] = useState<File | null>(null);
  const { registerUser } = useAuth();

  function handleImageChange(e: React.ChangeEvent<HTMLInputElement>) {
    if (e.target.files) {
      const image = e.target.files[0];
      if (image.size > 2 * 1024 * 1024) {
        toast.error('El archivo es demasiado grande (máximo 2MB)');
        return;
      }
      setImage(image);
    }
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await registerUser(name, email, password, image);
    } catch (error) {
      toast.error('Error al registrarse. Intenta nuevamente.');
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
                <Label htmlFor="name">Name</Label>
                <div className="relative flex items-center">
                  <IdCard className="text-muted-foreground absolute left-3 h-5 w-5" />
                  <Input
                    className="pl-10"
                    id="name"
                    type="name"
                    placeholder="Tu nombre y apellidos."
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
              <div className="grid gap-3">
                <div className="flex items-center">
                  <Label htmlFor="image">Foto de Perfil</Label>
                </div>
                <div className="relative flex items-center">
                  <Image className="text-muted-foreground absolute left-3 h-5 w-5" />
                  <Input
                    className="pl-10"
                    id="image"
                    name="image"
                    type="file"
                    accept="image/*"
                    onChange={handleImageChange}
                  />
                </div>
              </div>
              <div className="flex flex-col gap-3">
                <Button type="submit" className="w-full">
                  Registrarse
                </Button>
              </div>
            </div>
            <div className="mt-6 flex flex-col gap-3">
              <div className="relative">
                <div className="absolute inset-0 flex items-center">
                  <span className="w-full border-t"></span>
                </div>
                <div className="relative flex justify-center text-xs uppercase">
                  <span className="bg-background text-muted-foreground px-2">O regístrate con</span>
                </div>
              </div>
              <div className="flex justify-center">
                <GoogleLoginButton text="signup_with" />
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
