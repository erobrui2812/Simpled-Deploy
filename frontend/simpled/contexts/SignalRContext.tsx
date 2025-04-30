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

const handleBoardInvitation = (data: {
  boardName: string;
  role: string;
  invitationToken: string;
}) => {
  toast.info(`📩 Invitación al tablero "${data.boardName}" como ${data.role}`, {
    toastId: `board-invite-${data.invitationToken}`,
    onClick: () => {
      window.location.href = `/invitations/${data.invitationToken}`;
    },
  });
};

const handleTeamInvitation = (data: {
  teamName: string;
  role: string;
  invitationToken: string;
}) => {
  toast.info(`📩 Has sido invitado al equipo "${data.teamName}" como ${data.role}`, {
    toastId: `team-invite-${data.invitationToken}`,
    onClick: () => {
      window.location.href = `/equipos/invitacion/${data.invitationToken}`;
    },
  });
};

const handleBoardUpdated = (boardId: string) => {
  console.log('Tablero actualizado:', boardId);
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
    };
  }, [auth.token]);

  const contextValue = useMemo(
    () => ({ connection: connectionRef.current }),
    [connectionRef.current],
  );

  return <SignalRContext.Provider value={contextValue}>{children}</SignalRContext.Provider>;
};
