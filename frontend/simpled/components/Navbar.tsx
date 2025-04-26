'use client';

import { DarkModeToggle } from '@/components/DarkModeToggle';
import { useAuth } from '@/contexts/AuthContext';
import React, { useEffect, useState } from 'react';
import IconLink from '@/components/IconLink';
import { Home, Info, Layers, LogIn, LogOut, Menu, User, X } from 'lucide-react';
import { Sheet, SheetContent, SheetTrigger } from '@/components/ui/sheet';
import { Button } from '@/components/ui/button';
import Link from 'next/link';

export default function Navbar() {
  const [mostrarLogin, setMostrarLogin] = useState(true);
  const { isAuthenticated, logout, auth } = useAuth();
  useEffect(() => {
    if (isAuthenticated) {
      setMostrarLogin(false);
    }
  }, [isAuthenticated]);

  const NavItems = (className: string) => {
    return (
      <nav className={`flex ${className} gap-4`}>
        <IconLink href="/" icon={<Home className="size-4" />}>
          Inicio
        </IconLink>
        <IconLink href="/nosotros" icon={<Info className="size-4" />}>
          Nosotros
        </IconLink>
        <IconLink href="/tableros" icon={<Layers className="size-4" />}>
          Tableros
        </IconLink>
        {mostrarLogin ? (
          <IconLink href="/login" icon={<LogIn className="size-4" />}>
            Login
          </IconLink>
        ) : (
          <>
            <IconLink href={`/perfil/${auth.id}`} icon={<User className="size-4" />}>
              Perfil
            </IconLink>
            <button
              onClick={logout}
              className="hover:bg-accent flex items-center gap-2 rounded-md px-3 py-2 text-sm font-medium"
            >
              <LogOut className="size-4" />
              <span>Cerrar sesión</span>
            </button>
          </>
        )}
      </nav>
    );
  };

  return (
    <div className="flex items-center justify-between border-b-[0.5] p-4">
      <h1 className="text-4xl font-bold transition-transform duration-300 hover:scale-105 hover:rotate-5">
        <Link href={'/'}>Simpled.</Link>
      </h1>

      <div className="hidden items-center gap-4 font-semibold md:flex">
        {NavItems('items-center')}
        <DarkModeToggle />
      </div>

      <Sheet>
        <SheetTrigger asChild className="md:hidden">
          <Button variant="ghost" size="icon">
            <Menu className="h-5 w-5" />
          </Button>
        </SheetTrigger>

        <SheetContent side="right" className="flex flex-col">
          <div className="mt-6"></div>
          {NavItems('flex-col m-4')}
          <div className="mt-auto flex items-center justify-between pt-4">
            <DarkModeToggle />
          </div>
        </SheetContent>
      </Sheet>
    </div>
  );
}
