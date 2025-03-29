"use client";

import { cn } from "@/lib/utils"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Checkbox } from "@/components/ui/checkbox";
import { useState } from "react";
import { useAuth } from "@/contexts/AuthContext";

export function LoginForm({
  className,
  ...props
}: React.ComponentProps<"div">) {

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [mantenerSesion, setMantenerSesion] = useState(false);
  const { iniciarSesion } = useAuth();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await iniciarSesion(email, password, mantenerSesion);
    } catch (error) {
      console.error("Error al iniciar sesión:", error);
    }
  };

  return (
    <div className={cn("flex flex-col gap-6", className)} {...props}>
      <Card>
        <CardHeader>
          <CardTitle>Inicia sesión</CardTitle>
          <CardDescription>
            Escribe tu correo electrónico para iniciar sesión
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit}>
            <div className="flex flex-col gap-6">
              <div className="grid gap-3">
                <Label htmlFor="email">Email</Label>
                <Input
                  id="email"
                  type="email"
                  placeholder="email@dominio.com"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                />
              </div>
              <div className="grid gap-3">
                <div className="flex items-center">
                  <Label htmlFor="password">Contraseña</Label>
                </div>
                <Input 
                  id="password" 
                  type="password" 
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required 
                />
              </div>
              <div className="flex items-center gap-3">
                <Checkbox
                  id="mantenerSesion"
                  checked={mantenerSesion}
                  onCheckedChange={(checked: boolean) => setMantenerSesion(checked)}
                />
                <Label htmlFor="mantenerSesion">Mantener sesión iniciada</Label>
              </div>
              <div className="flex flex-col gap-3">
                <Button type="submit" className="w-full">
                  Iniciar sesión
                </Button>
              </div>
            </div>
            <div className="mt-4 text-center text-sm">
              ¿No tienes cuenta?{" "}
              <a href="/registro" className="underline underline-offset-4">
                Regístrate
              </a>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}