'use client';

import * as signalR from '@microsoft/signalr';
import React, { createContext, useContext, useEffect, useMemo, useState } from 'react';
import { toast } from 'react-toastify';
import { useAuth } from './AuthContext';

type SignalRContextType = {
  connection: signalR.HubConnection | null;
};

const SignalRContext = createContext<SignalRContextType>({ connection: null });

export const useSignalR = () => useContext(SignalRContext);

export const SignalRProvider = ({ children }: { children: React.ReactNode }) => {
  const { auth } = useAuth();
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);

  useEffect(() => {
    if (!auth.token) return;
    if (connection) return;

    const conn = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5193/hubs/board', {
        accessTokenFactory: () => auth.token!,
      })
      .withAutomaticReconnect()
      .build();

    conn.onreconnecting((error) => {
      console.warn('SignalR reconectando', error);
      toast.info('Reconectando a notificaciones…', {
        toastId: 'sigr-reconnecting',
      });
    });
    conn.onreconnected(() => {
      toast.dismiss('sigr-reconnecting');
      toast.success('Reconectado a notificaciones', {
        toastId: 'sigr-reconnected',
      });
    });
    conn.onclose((error) => {
      console.error('SignalR cerrado', error);
      toast.error('Se perdió conexión de notificaciones', {
        toastId: 'sigr-closed',
      });
    });

    conn.on('InvitationReceived', (data) => {
      toast.info(`📩 Invitación al tablero "${data.boardName}" como ${data.role}`, {
        toastId: `board-invite-${data.invitationToken}`,
      });
    });
    conn.on('TeamInvitationReceived', (data) => {
      toast.info(`📩 Invitado al equipo "${data.teamName}" como ${data.role}`, {
        toastId: `team-invite-${data.invitationToken}`,
      });
    });
    conn.on('BoardUpdated', (_boardId, action, payload) => {
      console.log('📡 BoardUpdated', { action, payload });
    });

    conn
      .start()
      .then(() => {
        setConnection(conn);
        console.log('✅ SignalR conectado');
      })
      .catch((err) => {
        console.error('❌ Error al conectar SignalR', err);
        toast.error('No se pudo conectar a notificaciones');
      });

    const handleUnload = () => conn.stop();
    window.addEventListener('beforeunload', handleUnload);

    return () => {
      window.removeEventListener('beforeunload', handleUnload);
    };
  }, [auth.token, connection]);

  useEffect(() => {
    if (!auth.token && connection) {
      connection.stop();
      setConnection(null);
    }
  }, [auth.token, connection]);

  const value = useMemo(() => ({ connection }), [connection]);

  return <SignalRContext.Provider value={value}>{children}</SignalRContext.Provider>;
};
