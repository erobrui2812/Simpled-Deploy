"use client"

import { useEffect, useState } from "react"
import { toast } from "react-toastify"
import "react-toastify/dist/ReactToastify.css"
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import Link from "next/link"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { useAuth } from "@/contexts/AuthContext"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { error } from "console"

const API_URL = "http://localhost:5193"

type User = {
  id: string
  name: string
  email: string
  imageUrl: string
  isOnline: boolean
  webRole: number
  isBanned: boolean
}

export default function AdminPage() {
  const [users, setUsers] = useState<User[]>([])
  const [loading, setLoading] = useState(true)
  const { auth } = useAuth()

  const fetchUsers = async () => {
    try {
      const response = await fetch(`${API_URL}/api/users`, {
        headers: {
          Authorization: `Bearer ${auth.token}`,
        },
      })

      if (!response.ok) {
        throw new Error("Error al obtener usuarios")
      }

      const data = await response.json()
      setUsers(data)
    } catch (error) {
      toast.error("No se pudieron cargar los usuarios.")
    } finally {
      setLoading(false)
    }
  }

  const changeUserRole = async (userId: string, newRole: number) => {
    try {
      if (userId === auth.id) {
        throw new Error("No puedes cambiar tu propio rol.")
      }
      const response = await fetch(`${API_URL}/api/users/${userId}/role?role=${newRole}`, {
        method: "PUT",
        headers: {
          Authorization: `Bearer ${auth.token}`,
        },
      })

      if (!response.ok) {
        throw new Error("Error al cambiar el rol")
      }

      setUsers(users.map((user) => (user.id === userId ? { ...user, role: newRole } : user)))

      toast.success("Rol de usuario actualizado correctamente.")
    } catch (error) {
      toast.error("No se pudo cambiar el rol del usuario.")
    }
  }

  const toggleUserBan = async (userId: string, isBanned: boolean) => {
    try {
      if (userId === auth.id) {
        throw new Error("No puedes banearte a ti mismo.")
      }

      const response = await fetch(`${API_URL}/api/users/${userId}/ban?isBanned=${isBanned}`, {
        method: "PUT",
        headers: {
          Authorization: `Bearer ${auth.token}`,
        },
      })

      if (!response.ok) {
        throw new Error("Error al actualizar el estado de baneo")
      }

      setUsers(users.map((user) => (user.id === userId ? { ...user, isBanned } : user)))

      toast.success(`Usuario ${isBanned ? "baneado" : "desbaneado"} correctamente.`)
    } catch (error) {
      toast.error("No se pudo actualizar el estado de baneo del usuario.")
    }
  }

  useEffect(() => {
    fetchUsers()
  }, [])

  if (loading) {
    return (
      <div className="flex h-screen flex-col items-center justify-center">
        <div className="border-foreground h-12 w-12 animate-spin rounded-full border-b-2" />
        <p className="text-foreground mt-4 text-sm">Cargando usuarios...</p>
      </div>
    )
  }

  return (
    <div className="container mx-auto py-6">
      <Card>
        <CardHeader>
          <CardTitle>Administración de Usuarios</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Usuario</TableHead>
                  <TableHead>Rol</TableHead>
                  <TableHead>Estado</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {users.map((user) => (
                  <TableRow key={user.id}>
                    <TableCell>
                      <div className="flex items-center space-x-4">
                        <Avatar>
                          <AvatarImage src={`${API_URL}${user.imageUrl}`} alt={user.name} />
                          <AvatarFallback>{user.name.substring(0, 2).toUpperCase()}</AvatarFallback>
                        </Avatar>
                        <div>
                          <div className="flex items-center space-x-2">
                            <p className="font-medium">{user.name}</p>
                            {user.isBanned && (
                              <span className="inline-flex items-center rounded-md bg-red-50 px-2 py-1 text-xs font-medium text-red-700 ring-1 ring-inset ring-red-600/10">
                                Baneado
                              </span>
                            )}
                          </div>
                          <Link href={`/perfil/${user.id}`} className="text-sm text-muted-foreground hover:underline">
                            {user.id}
                          </Link>
                        </div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <Select defaultValue={`${user.webRole}`} onValueChange={(value) => changeUserRole(user.id, Number(value))}>
                        <SelectTrigger className="w-[180px]">
                          <SelectValue placeholder="Seleccionar rol" />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="1">Administrador</SelectItem>
                          <SelectItem value="0">Usuario</SelectItem>
                        </SelectContent>
                      </Select>
                    </TableCell>
                    <TableCell>
                      <button
                        onClick={() => toggleUserBan(user.id, !user.isBanned)}
                        className={`px-3 py-1 rounded-md text-sm font-medium ${
                          user.isBanned
                            ? "bg-green-100 text-green-700 hover:bg-green-200"
                            : "bg-red-100 text-red-700 hover:bg-red-200"
                        }`}
                      >
                        {user.isBanned ? "Desbanear" : "Banear"}
                      </button>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
