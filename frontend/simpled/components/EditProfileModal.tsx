'use client';

import type React from 'react';

import { useState } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
  SheetFooter,
} from '@/components/ui/sheet';
import { Image } from 'lucide-react';
import { toast } from 'react-toastify';

interface User {
  id: string;
  name: string;
  email: string;
  photo: string;
}

interface EditProfileModalProps {
  isOpen: boolean;
  onClose: () => void;
  user: User;
}

export default function EditProfileModal({ isOpen, onClose, user }: EditProfileModalProps) {
  const [formData, setFormData] = useState({
    name: user.name,
    email: user.email,
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    toast.info('Funciona');
    await new Promise((resolve) => setTimeout(resolve, 500));
    onClose();
  };

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent>
        <SheetHeader>
          <SheetTitle>Editar Perfil</SheetTitle>
          <SheetDescription>Actualiza tu información de perfil</SheetDescription>
        </SheetHeader>

        <form onSubmit={handleSubmit} className="space-y-4 px-4 font-medium">
          <div className="space-y-2">
            <Label htmlFor="name">Nombre</Label>
            <Input id="name" name="name" value={formData.name} onChange={handleChange} />
          </div>

          <div className="space-y-2">
            <Label htmlFor="email">Email</Label>
            <Input
              id="email"
              name="email"
              type="email"
              value={formData.email}
              onChange={handleChange}
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="photo">Foto de Perfil</Label>
            <div className="flex items-center gap-4">
              <Image className="h-16 w-16" />
              <Input id="photo" name="photo" type="file" accept="image/*" />
            </div>
          </div>

          <SheetFooter className="pt-4">
            <Button type="submit">Guardar Cambios</Button>
          </SheetFooter>
        </form>
      </SheetContent>
    </Sheet>
  );
}
