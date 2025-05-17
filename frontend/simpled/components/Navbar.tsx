'use client';

import { DarkModeToggle } from '@/components/DarkModeToggle';
import IconLink from '@/components/IconLink';
import { Button } from '@/components/ui/button';
import { Sheet, SheetContent, SheetTrigger } from '@/components/ui/sheet';
import { useAuth } from '@/contexts/AuthContext';
import {
  Bell,
  ChevronDown,
  Home,
  Info,
  Layers,
  LogOut,
  Menu,
  PieChart,
  User,
  Users,
  LogIn,
} from 'lucide-react';
import Link from 'next/link';
import { useEffect, useState } from 'react';
import InvitationsModal from './InvitationModal';

interface NavItemsProps {
  readonly mostrarLogin: boolean;
  readonly authId: string;
  readonly logout: () => void;
  readonly onShowInvitations: () => void;
  readonly className?: string;
}

function NavItems({
  mostrarLogin,
  authId,
  logout,
  onShowInvitations,
  className = '',
}: NavItemsProps) {
  return (
    <nav className={`flex ${className} gap-4`}>
      <IconLink href="/" icon={<Home className="size-4" />}>
        Inicio
      </IconLink>
      <IconLink href="/nosotros" icon={<Info className="size-4" />}>
        Nosotros
      </IconLink>
      {mostrarLogin ? (
        <IconLink href="/login" icon={<LogIn className="size-4" />}>
          Login
        </IconLink>
      ) : (
        <>
          <IconLink href="/dashboard" icon={<PieChart className="size-4" />}>
            Dashboard
          </IconLink>
          <IconLink href="/tableros" icon={<Layers className="size-4" />}>
            Tableros
          </IconLink>
          <IconLink href="/equipos" icon={<Users className="size-4" />}>
            Equipos
          </IconLink>

          <div className="group relative">
            <Button variant="ghost" size="sm" className="flex h-9 items-center gap-2">
              <User className="size-4" />
              <span>Mi cuenta</span>
              <ChevronDown className="size-4 transition-transform duration-200 ease-in-out group-hover:rotate-180" />
            </Button>
            <div className="bg-popover ring-opacity-5 invisible absolute right-0 z-10 mt-1 w-60 origin-top-right rounded-md opacity-0 shadow-lg ring-1 ring-black transition-all duration-200 ease-in-out group-hover:visible group-hover:opacity-100 focus:outline-none">
              <div className="py-1">
                <Link
                  href={`/perfil/${authId}`}
                  className="hover:bg-accent hover:text-accent-foreground flex w-full cursor-pointer items-center gap-2 rounded-md px-4 py-2 text-sm"
                >
                  <User className="size-4 min-w-4" />
                  <span className="flex-grow">Perfil</span>
                </Link>
                <button
                  onClick={onShowInvitations}
                  className="hover:bg-accent hover:text-accent-foreground flex w-full cursor-pointer items-center gap-2 rounded-md px-4 py-2 text-left text-sm"
                >
                  <Bell className="size-4 min-w-4" />
                  <span className="flex-grow">Invitaciones</span>
                </button>
                <div className="border-border border-t"></div>
                <button
                  onClick={logout}
                  className="text-destructive hover:bg-accent hover:text-destructive flex w-full cursor-pointer items-center gap-2 rounded-md px-4 py-2 text-left text-sm"
                >
                  <LogOut className="size-4 min-w-4" />
                  <span className="flex-grow">Cerrar sesión</span>
                </button>
              </div>
            </div>
          </div>
        </>
      )}
    </nav>
  );
}

export default function Navbar() {
  const [mostrarLogin, setMostrarLogin] = useState(true);
  const { isAuthenticated, logout, auth } = useAuth();
  const [showInvitations, setShowInvitations] = useState(false);
  const [mobileDropdownOpen, setMobileDropdownOpen] = useState(false);

  useEffect(() => {
    setMostrarLogin(!isAuthenticated);
  }, [isAuthenticated]);

  return (
    <div className="bg-background/80 sticky top-0 z-10 flex items-center justify-between border-b p-4 backdrop-blur-sm">
      <h1 className="text-4xl font-bold transition-all duration-300 hover:scale-105 hover:rotate-1">
        <Link href="/">Simpled.</Link>
      </h1>

      <div className="hidden items-center gap-4 font-semibold md:flex">
        <NavItems
          mostrarLogin={mostrarLogin}
          authId={auth.id ?? ''}
          logout={logout}
          onShowInvitations={() => setShowInvitations(true)}
          className="items-center"
        />
        <DarkModeToggle />
      </div>

      <Sheet>
        <SheetTrigger asChild className="md:hidden">
          <Button variant="ghost" size="icon" aria-label="Menu">
            <Menu className="h-5 w-5" />
          </Button>
        </SheetTrigger>

        <SheetContent side="right" className="flex flex-col">
          <div className="mt-6"></div>
          <nav className={`m-4 flex flex-col gap-4`}>
            <IconLink href="/" icon={<Home className="size-4" />}>
              Inicio
            </IconLink>
            <IconLink href="/nosotros" icon={<Info className="size-4" />}>
              Nosotros
            </IconLink>
            {mostrarLogin ? (
              <IconLink href="/login" icon={<LogIn className="size-4" />}>
                Login
              </IconLink>
            ) : (
              <>
                <IconLink href="/dashboard" icon={<PieChart className="size-4" />}>
                  Dashboard
                </IconLink>
                <IconLink href="/tableros" icon={<Layers className="size-4" />}>
                  Tableros
                </IconLink>
                <IconLink href="/equipos" icon={<Users className="size-4" />}>
                  Equipos
                </IconLink>

                <div className="relative">
                  <Button
                    variant="ghost"
                    size="sm"
                    className="flex h-9 w-full items-center justify-between gap-2"
                    onClick={(e) => {
                      e.preventDefault();
                      setMobileDropdownOpen(!mobileDropdownOpen);
                    }}
                  >
                    <div className="flex items-center gap-2">
                      <User className="size-4" />
                      <span>Mi cuenta</span>
                    </div>
                    <ChevronDown
                      className={`size-4 transition-transform duration-200 ease-in-out ${mobileDropdownOpen ? 'rotate-180' : ''}`}
                    />
                  </Button>
                  {mobileDropdownOpen && (
                    <div className="bg-popover border-border mt-1 w-full rounded-md border shadow-md">
                      <div className="py-1">
                        <Link
                          href={`/perfil/${auth.id ?? ''}`}
                          className="hover:bg-accent hover:text-accent-foreground flex w-full cursor-pointer items-center gap-2 rounded-md px-4 py-2 text-sm"
                        >
                          <User className="size-4 min-w-4" />
                          <span className="flex-grow">Perfil</span>
                        </Link>
                        <button
                          onClick={() => {
                            setShowInvitations(true);
                            setMobileDropdownOpen(false);
                          }}
                          className="hover:bg-accent hover:text-accent-foreground flex w-full cursor-pointer items-center gap-2 rounded-md px-4 py-2 text-left text-sm"
                        >
                          <Bell className="size-4 min-w-4" />
                          <span className="flex-grow">Invitaciones</span>
                        </button>
                        <div className="border-border border-t"></div>
                        <button
                          onClick={() => {
                            logout();
                            setMobileDropdownOpen(false);
                          }}
                          className="text-destructive hover:bg-accent hover:text-destructive flex w-full cursor-pointer items-center gap-2 rounded-md px-4 py-2 text-left text-sm"
                        >
                          <LogOut className="size-4 min-w-4" />
                          <span className="flex-grow">Cerrar sesión</span>
                        </button>
                      </div>
                    </div>
                  )}
                </div>
              </>
            )}
          </nav>
          <div className="mt-auto flex items-center justify-between pt-4">
            <DarkModeToggle />
          </div>
        </SheetContent>
      </Sheet>

      {showInvitations && <InvitationsModal onClose={() => setShowInvitations(false)} />}
    </div>
  );
}
