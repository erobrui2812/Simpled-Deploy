import type { Metadata } from 'next';
import { Figtree } from 'next/font/google';
import './globals.css';

import Footer from '@/components/Footer';
import Navbar from '@/components/Navbar';

import { AuthProvider } from '@/contexts/AuthContext';
import { BoardsProvider } from '@/contexts/BoardsContext';
import { SignalRProvider } from '@/contexts/SignalRContext';
import { TeamsProvider } from '@/contexts/TeamsContext';

import { Slide, ToastContainer } from 'react-toastify';

const figtree = Figtree({
  variable: '--font-figtree',
  subsets: ['latin'],
  weight: ['300', '400', '500', '600', '700', '800', '900'],
  style: ['normal', 'italic'],
});

export const metadata: Metadata = {
  title: 'Simpled - Organización Colaborativa',
  description: 'Plataforma estilo Trello/Notion para gestionar tareas en equipo.',
};

export default function RootLayout({ children }: { readonly children: React.ReactNode }) {
  return (
    <html lang="es">
      <body className={`${figtree.variable} antialiased`}>
        <AuthProvider>
          <BoardsProvider>
            <TeamsProvider>
              <SignalRProvider>
                <ToastContainer
                  position="top-right"
                  autoClose={5000}
                  hideProgressBar={false}
                  newestOnTop={false}
                  closeOnClick={false}
                  rtl={false}
                  pauseOnFocusLoss
                  draggable={false}
                  pauseOnHover
                  theme="dark"
                  transition={Slide}
                />
                <Navbar />
                {children}
                <Footer />
              </SignalRProvider>
            </TeamsProvider>
          </BoardsProvider>
        </AuthProvider>
      </body>
    </html>
  );
}
