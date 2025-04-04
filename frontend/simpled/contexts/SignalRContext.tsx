"use client";

import * as signalR from "@microsoft/signalr";
import React, { createContext, useContext, useEffect, useRef } from "react";
import { toast } from "react-toastify";
import { useAuth } from "./AuthContext";

type SignalRContextType = {
  connection: signalR.HubConnection | null;
};

const SignalRContext = createContext<SignalRContextType>({ connection: null });

export const useSignalR = () => useContext(SignalRContext);

export const SignalRProvider = ({
  children,
}: {
  children: React.ReactNode;
}) => {
  const { auth } = useAuth();
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    if (!auth.token) return;

    const connect = async () => {
      const connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7177/hubs/board", {
          accessTokenFactory: () => auth.token!,
        })
        .withAutomaticReconnect()
        .build();

      connection.on("InvitationReceived", (message: string) => {
        toast.info(message, { toastId: "invitation-toast" });
      });

      connection.on("BoardUpdated", (boardId: string) => {
        console.log("Tablero actualizado:", boardId);
      });

      try {
        await connection.start();
        console.log("✅ Conectado a SignalR");
        connectionRef.current = connection;
      } catch (err) {
        console.error("❌ Error al conectar a SignalR", err);
      }
    };

    connect();

    return () => {
      connectionRef.current?.stop();
    };
  }, [auth.token]);

  const contextValue = React.useMemo(
    () => ({ connection: connectionRef.current }),
    [connectionRef.current]
  );

  return (
    <SignalRContext.Provider value={contextValue}>
      {children}
    </SignalRContext.Provider>
  );
};
