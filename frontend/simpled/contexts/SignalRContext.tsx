'use client';

import * as signalR from '@microsoft/signalr';
import React, { createContext, useContext, useEffect, useMemo, useRef } from 'react';
import { toast } from 'react-toastify';
import { useAuth } from './AuthContext';

type SignalRContextType = {
  connection: signalR.HubConnection | null;
};

const SignalRContext = createContext<SignalRContextType>({ connection: null });

export const useSignalR = () => useContext(SignalRContext);

const showToast = (() => {
  const cache = new Set<string>();
  return (message: string, id: string) => {
    if (cache.has(id)) return;
    cache.add(id);
    toast.info(message, {
      toastId: id,
      onClose: () => cache.delete(id),
    });
  };
})();

const handleBoardInvitation = (data: {
  boardName: string;
  role: string;
  invitationToken: string;
}) => {
  showToast(
    `📩 Invitación al tablero "${data.boardName}" como ${data.role}`,
    `board-invite-${data.invitationToken}`,
  );
};

const handleTeamInvitation = (data: {
  teamName: string;
  role: string;
  invitationToken: string;
}) => {
  showToast(
    `📩 Has sido invitado al equipo "${data.teamName}" como ${data.role}`,
    `team-invite-${data.invitationToken}`,
  );
};

const handleBoardUpdated = (boardId: string, action: string, payload: any) => {
  console.log('[SignalR] BoardUpdated', { boardId, action, payload });
};

export const SignalRProvider = ({ children }: { children: React.ReactNode }) => {
  const { auth } = useAuth();
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    if (!auth.token || connectionRef.current) return;

    const connect = async () => {
      const conn = new signalR.HubConnectionBuilder()
        .withUrl('http://localhost:5193/hubs/board', {
          accessTokenFactory: () => auth.token!,
        })
        .withAutomaticReconnect()
        .build();

      conn.on('InvitationReceived', handleBoardInvitation);
      conn.on('TeamInvitationReceived', handleTeamInvitation);
      conn.on('BoardUpdated', handleBoardUpdated);

      try {
        await conn.start();
        connectionRef.current = conn;
        console.log('✅ SignalR conectado');
      } catch (err) {
        console.error('❌ Error al conectar SignalR', err);
      }
    };

    connect();

    return () => {
      connectionRef.current?.stop();
      connectionRef.current = null;
    };
  }, [auth.token]);

  const contextValue = useMemo(
    () => ({ connection: connectionRef.current }),
    [connectionRef.current],
  );

  return <SignalRContext.Provider value={contextValue}>{children}</SignalRContext.Provider>;
};
