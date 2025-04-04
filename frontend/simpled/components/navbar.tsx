"use client";

import { DarkModeToggle } from "@/components/darkmode-toggle";
import { useAuth } from "@/contexts/AuthContext";
import React, { useEffect, useState } from "react";
import IconLink from "@/components/IconLink";
import { Home, Info, Layers, LogIn, LogOut, Menu, User, X } from "lucide-react";
import { Sheet, SheetContent, SheetTrigger } from "@/components/ui/sheet";
import { Button } from "@/components/ui/button";

export default function Navbar() {
  const [mostrarLogin, setMostrarLogin] = useState(true);
  const { isAuthenticated, cerrarSesion } = useAuth();
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
          <IconLink href="/login" icon={<LogIn className="size-4" />}>Login</IconLink>
        ) : (
          <>
            <IconLink href="/perfil" icon={<User className="size-4" />}>
              Perfil
            </IconLink>
            <button
              onClick={cerrarSesion}
              className="flex items-center gap-2 px-3 py-2 text-sm font-medium rounded-md hover:bg-accent"
            >
              <LogOut className="size-4" />
              <span>Cerrar sesión</span>
            </button>
          </>
        )}
      </nav>
    );
  }

  return (
    <div className="flex justify-between items-center p-4 border-b-[0.5]">
      <h1 className="font-bold text-4xl">Simpled.</h1>

      <div className="hidden md:flex items-center gap-4 font-semibold">
        {NavItems("items-center")}
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
          {NavItems("flex-col m-4")}
          <div className="mt-auto flex items-center justify-between pt-4">
            <DarkModeToggle />
          </div>
        </SheetContent>
      </Sheet>
    </div>
  );
}
