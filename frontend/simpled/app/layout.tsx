import type { Metadata } from 'next';
import { Figtree } from 'next/font/google';
import type React from 'react';
import './globals.css';

import Footer from '@/components/Footer';
import Navbar from '@/components/Navbar';

import { AuthProvider } from '@/contexts/AuthContext';
import { BoardsProvider } from '@/contexts/BoardsContext';
import { SignalRProvider } from '@/contexts/SignalRContext';
import { TeamsProvider } from '@/contexts/TeamsContext';

import { Bounce, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const figtree = Figtree({
  variable: '--font-figtree',
  subsets: ['latin'],
  weight: ['300', '400', '500', '600', '700', '800', '900'],
  style: ['normal', 'italic'],
});

export const metadata: Metadata = {
  title: 'Simpled - Organización Colaborativa',
  description: 'Plataforma de gestión de tareas y proyectos en equipo - Estilo Kanban/Trello.',
  keywords: 'kanban, trello, gestión de proyectos, tareas, colaboración, equipo',
};

export default function RootLayout({ children }: { readonly children: React.ReactNode }) {
  return (
    <html lang="es" suppressHydrationWarning>
      <body className={`${figtree.variable} flex min-h-screen flex-col antialiased`}>
        <AuthProvider>
          <BoardsProvider>
            <TeamsProvider>
              <SignalRProvider>
                <ToastContainer
                  position="top-right"
                  autoClose={3000}
                  hideProgressBar={false}
                  newestOnTop
                  closeOnClick
                  rtl={false}
                  pauseOnFocusLoss
                  draggable
                  pauseOnHover
                  theme="light"
                  transition={Bounce}
                  className="toast-container"
                />
                <Navbar />
                <main className="container mx-auto flex-1 p-4">{children}</main>
                <Footer />
              </SignalRProvider>
            </TeamsProvider>
          </BoardsProvider>
        </AuthProvider>
      </body>
    </html>
  );
}
