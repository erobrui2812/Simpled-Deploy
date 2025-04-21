"use client"

import Image from "next/image"
import { Button } from "@/components/ui/button"
import { useState } from "react"
import EditProfileModal from "@/components/EditProfileModal"
import { Card, CardContent } from "@/components/ui/card"

interface User {
  id: string
  name: string
  email: string
  photo: string
  isOnline: boolean
}

interface ProfileHeaderProps {
  user: User
  isOwner: boolean
}

export default function ProfileHeader({ user, isOwner }: ProfileHeaderProps) {
  const [isModalOpen, setIsModalOpen] = useState(false)

  return (
    <Card className="h-full">
      <CardContent className="flex flex-col md:flex-row items-center">
        <div className="relative">
          <div
            className={`absolute w-full h-full rounded-full border-2 ${user.isOnline ? "border-green-500" : "border-gray-400"}`}
          ></div>
          <div className="relative w-32 h-32 rounded-full overflow-hidden">
            <Image src={user.photo || "/placeholder.svg"} alt={user.name} fill className="object-cover" />
          </div>
          <div
            className={`absolute bottom-1 right-1 w-4 h-4 rounded-full ${user.isOnline ? "bg-green-500" : "bg-gray-400"} border-2 border-white dark:border-gray-800`}
          ></div>
        </div>

        <div className="md:ml-8 mt-4 md:mt-0 text-center md:text-left flex-1">
          <h1 className="text-2xl font-bold">{user.name}</h1>
          <p className="text-gray-600 dark:text-gray-400">{user.email}</p>
        </div>

        {isOwner && (
          <div className="mt-4 md:mt-0">
            <Button onClick={() => setIsModalOpen(true)}>Editar Perfil</Button>
          </div>
        )}

        {isOwner && <EditProfileModal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} user={user} />}
      </CardContent>
    </Card>
  )
}
