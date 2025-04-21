"use client";
import { useRouter } from "next/navigation";
import React, { createContext, useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";

const API_URL = "http://localhost:5193/";

type User = {
  name: string;
  email: string;
  photo: string;
  isOnline: boolean;
  achievementsCompleted: number;
  teams: Team[];
};

interface Team {
  id: string;
  name: string;
  role: string;
}

type AuthContextType = {
  auth: { token: string | null, id: string | null };
  iniciarSesion: (
    email: string,
    password: string,
    mantenerSesion: boolean
  ) => Promise<void>;
  userData: User | null;
  registrarUsuario: (email: string, password: string) => Promise<void>;
  cerrarSesion: () => void;
  isAuthenticated: boolean;
  fetchUserProfile: (userId: string) => Promise<User | null>;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [auth, setAuth] = useState<{ token: string | null, id: string | null }>({ token: null, id: null });
  const [isAuthenticated, setAuthenticated] = useState(false);
  const router = useRouter();
  const [userData, setUserData] = useState<User | null>(null);

  useEffect(() => {
    const token =
      typeof window !== "undefined"
        ? sessionStorage.getItem("token") || localStorage.getItem("token")
        : null;

    const id =
      typeof window !== "undefined"
        ? sessionStorage.getItem("userId") || localStorage.getItem("userId")
        : null;

    if (token && id) {
      setAuth({ token, id });
    }
  }, []);

  useEffect(() => {
    if (auth.token) {
      setAuthenticated(true);

    } else {
      setAuthenticated(false);
    }
  }, [auth.token]);

  const fetchUserProfile = async (userId: string) => {
    try {
      const response = await fetch(`${API_URL}api/Users/${userId}`);
      const data = await response.json();
      return data;
    } catch (error) {
      console.error('Error al obtener el perfil del usuario:', error);
      return null;
    }
  };

  useEffect(() => {
    const getUserData = async () => {
      if (auth.id && !userData) {
        const profile = await fetchUserProfile(auth.id);
        setUserData(profile);
      }
    };
    getUserData();
  }, [auth.id, userData]);  

  const iniciarSesion = async (
    email: string,
    password: string,
    mantenerSesion: boolean
  ) => {
    try {
      const response = await fetch(`${API_URL}api/Auth/login`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ email, password }),
      });

      if (!response.ok)
        throw new Error("Credenciales incorrectas o error en el servidor.");

      console.log("response", response);
      const { token, id } = await response.json();
      setAuth({ token, id });


      if (mantenerSesion) {
        localStorage.setItem("token", token);
        localStorage.setItem("userId", id);
      } else {
        sessionStorage.setItem("token", token);
        sessionStorage.setItem("userId", id);
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

      toast.success(
        "Usuario registrado correctamente. Ahora puedes iniciar sesión."
      );
      router.push("/login");
    } catch (error) {
      console.error("Error registrando usuario:", error);
      toast.error("Error al registrar usuario. Intenta nuevamente.");
    }
  };

  const cerrarSesion = () => {
    setAuth({ token: null, id: null });

    setUserData(null);

    sessionStorage.removeItem("userId");
    localStorage.removeItem("userId");

    localStorage.removeItem("token");
    sessionStorage.removeItem("token");

    setAuthenticated(false);

    router.push("/login");
    toast.info("Sesión cerrada.");
  };

  return (
    <AuthContext.Provider
      value={{
        auth,
        iniciarSesion,
        registrarUsuario,
        cerrarSesion,
        isAuthenticated,
        userData,
        fetchUserProfile
      }}
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
