import type { Metadata } from 'next';
import { Figtree } from 'next/font/google';
import './globals.css';

import Footer from '@/components/footer';
import Navbar from '@/components/Navbar';

import { AuthProvider } from '@/contexts/AuthContext';
import { BoardsProvider } from '@/contexts/BoardsContext';
import { SignalRProvider } from '@/contexts/SignalRContext';
import { GoogleOAuthProvider } from '@react-oauth/google';
import { Slide, ToastContainer } from 'react-toastify';

const figtree = Figtree({
  variable: '--font-figtree',
  subsets: ['latin'],
  weight: ['300', '400', '500', '600', '700', '800', '900'],
  style: ['normal', 'italic'],
});

const clientId = '234342175737-c4qr5thap0ub5pmprouikobsk4k8uml7.apps.googleusercontent.com';

export const metadata: Metadata = {
  title: 'Simpled - Organización Colaborativa',
  description: 'Plataforma estilo Trello/Notion para gestionar tareas en equipo.',
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <GoogleOAuthProvider clientId={clientId}>
      <AuthProvider>
        <BoardsProvider>
          <SignalRProvider>
            <html lang="es">
              <body className={`${figtree.variable} antialiased`}>
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
              </body>
            </html>
          </SignalRProvider>
        </BoardsProvider>
      </AuthProvider>
    </GoogleOAuthProvider>
  );
}
