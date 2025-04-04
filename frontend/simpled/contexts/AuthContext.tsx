"use client";
import React, { createContext, useCallback, useContext, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { toast } from "react-toastify";

const API_URL = "https://localhost:7177/";

type AuthContextType = {
  auth: { token: string | null };
  iniciarSesion: (email: string, password: string, mantenerSesion: boolean) => Promise<void>;
  registrarUsuario: (email: string, password: string) => Promise<void>;
  cerrarSesion: () => void;
  isAuthenticated: boolean;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [auth, setAuth] = useState<{ token: string | null }>({ token: null });
  const [isAuthenticated, setAuthenticated] = useState(false);
  const router = useRouter();

  useEffect(() => {
    const token =
      typeof window !== "undefined"
        ? sessionStorage.getItem("token") || localStorage.getItem("token")
        : null;
    setAuth({ token });
  }, []);

  useEffect(() => {
    if (auth.token) {
      setAuthenticated(true);
    } else {
      setAuthenticated(false);
    }
  }, [auth.token]);

  const iniciarSesion = async (email: string, password: string, mantenerSesion: boolean) => {
    try {
      const response = await fetch(`${API_URL}api/Auth/login`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ email, password }),
      });

      if (!response.ok) throw new Error("Credenciales incorrectas o error en el servidor.");

      const { token } = await response.json();
      setAuth({ token });

      if (mantenerSesion) {
        localStorage.setItem("token", token);
      } else {
        sessionStorage.setItem("token", token);
      }
      toast.success("Sesión iniciada correctamente.");
      router.push("/");
      setAuthenticated(true);
    } catch (error) {
      console.error("Error al iniciar sesión:", error);
      toast.error("Correo o contraseña incorrectos.");
    }
  };

  const registrarUsuario = async (email: string, password: string) => {
    try {
      const response = await fetch(`${API_URL}api/Users/register`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ email, password }),
      });

      if (!response.ok) throw new Error("Error al registrar usuario.");

      toast.success("Usuario registrado correctamente. Ahora puedes iniciar sesión.");
      router.push("/login");
    } catch (error) {
      console.error("Error registrando usuario:", error);
      toast.error("Error al registrar usuario. Intenta nuevamente.");
    }
  };

  const cerrarSesion = () => {
    setAuth({ token: null });
    localStorage.removeItem("token");
    sessionStorage.removeItem("token");
    setAuthenticated(false);
    router.push("/login");
    toast.info("Sesión cerrada.");
  };

  return (
    <AuthContext.Provider
      value={{ auth, iniciarSesion, registrarUsuario, cerrarSesion, isAuthenticated }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth debe usarse dentro de un AuthProvider");
  }
  return context;
};