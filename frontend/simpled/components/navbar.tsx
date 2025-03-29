"use client";

import { DarkModeToggle } from "@/components/darkmode-toggle";
import { useAuth } from "@/contexts/AuthContext";
import { useEffect, useState } from "react";

export default function Navbar() {
  const [mostrarLogin, setMostrarLogin] = useState(true);
  const [menuAbierto, setMenuAbierto] = useState(false);
  const { isAuthenticated, cerrarSesion } = useAuth();
  useEffect(() => {
    if (isAuthenticated) {
      setMostrarLogin(false);
    }
  }, [isAuthenticated]);

  return (
    <nav className="flex justify-between items-center p-4 border-b-[0.5]">
      <h1 className="font-bold text-4xl">Simpled.</h1>

      <div className="md:hidden">
        <button
          onClick={() => setMenuAbierto(!menuAbierto)}
          className="text-2xl"
        >
          {menuAbierto ? "X" : "☰"}
        </button>
      </div>

      <div className="hidden md:flex items-center gap-4 font-semibold">
        <div
          className={`grid ${
            mostrarLogin ? "grid-cols-4" : "grid-cols-5"
          } divide-x`}
        >
          <a href="/" className="text-lg px-4 text-center">
            Inicio
          </a>
          <a href="/nosotros" className="text-lg px-4 text-center">
            Nosotros
          </a>
          <a href="/boards" className="text-lg px-4 text-center">
            Tableros
          </a>
          {mostrarLogin ? (
            <a href="/login" className="text-lg px-4 text-center">
              Login
            </a>
          ) : (
            <>
              <a onClick={cerrarSesion} className="text-lg px-4 text-center">
                Cerrar sesión
              </a>
              <a href="/perfil" className="text-lg px-4 text-center">
                Perfil
              </a>
            </>
          )}
        </div>
        <DarkModeToggle />
      </div>

      <div
        className={`fixed inset-0 bg-background text-foreground bg-opacity-100 justify-center items-center transition-all duration-300 ${
          menuAbierto ? "flex" : "hidden"
        } md:hidden`}
      >
        <div className="text-4xl flex flex-col items-center justify-center">
          <div className="absolute top-8 right-8 text-center gap-3 scale-125 justify-center flex items-center">
            <DarkModeToggle />
            <button onClick={() => setMenuAbierto(false)}>X</button>
          </div>
          <div className="flex divide-y-2 flex-col">
            <a href="/" className="py-2 text-center">
              Inicio
            </a>
            <a href="/nosotros" className="py-2 text-center">
              Nosotros
            </a>
            <a href="/tableros" className="py-2 text-center">
              Tableros
            </a>
            {mostrarLogin ? (
              <a href="/login" className="py-2 text-center">
                Login
              </a>
            ) : (
              <>
                <a onClick={cerrarSesion} className="py-2 text-center">
                  Perfil
                </a>
                <a href="/perfil" className="py-2 text-center">
                  Perfil
                </a>
              </>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
}
