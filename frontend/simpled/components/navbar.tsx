"use client";

import { useEffect, useState } from "react";
import { DarkModeToggle } from "@/components/darkmode-toggle";

export default function Navbar() {
    const [mostrarLogin, setMostrarLogin] = useState(true);
    const [menuAbierto, setMenuAbierto] = useState(false);

    useEffect(() => {
        // Verificar si el usuario está autenticado
        // var user = null;
        // if (user) {
        //     setMostrarLogin(false);
        // }
    }, []);

    return (
        <nav className="flex justify-between items-center p-4 border-b-[0.5]flex-row">
            <h1 className="font-bold text-4xl">Simpled.</h1>

            <div className="md:hidden">
                <button
                    onClick={() => setMenuAbierto(!menuAbierto)}
                    className="text-2xl"
                >
                    {menuAbierto ? "X" : "☰"}
                </button>
            </div>

            <div className={`flex items-center gap-4 font-semibold ${menuAbierto ? 'md:flex' : 'hidden'} md:flex-row`}>
                <div className={`grid ${mostrarLogin ? 'grid-cols-4' : 'grid-cols-3'} divide-x`}>
                    <a href="/" className="text-lg px-4 text-center">Inicio</a>
                    <a href="/nosotros" className="text-lg px-4 text-center">Nosotros</a>
                    <a href="/tableros" className="text-lg px-4 text-center">Tableros</a>
                    {mostrarLogin ? (
                        <a href="/login" className="text-lg px-4 text-center">Login</a>
                    ) : null}
                </div>
                <DarkModeToggle />
            </div>

            <div className={`md:hidden ${menuAbierto ? 'flex' : 'hidden'} fixed inset-0 bg-background text-foreground bg-opacity-100 justify-center items-center`}>
                <div className="text-4xl flex flex-col items-center justify-center">
                    <div className="absolute top-8 right-8 text-center gap-3 scale-125 justify-center flex items-center">
                        <DarkModeToggle />
                        <button onClick={() => setMenuAbierto(false)}>
                            X
                        </button>
                    </div>
                    <div className="flex divide-y-2 flex-col">
                        <a href="/" className="py-2 text-center">Inicio</a>
                        <a href="/nosotros" className="py-2 text-center">Nosotros</a>
                        <a href="/tableros" className="py-2 text-center">Tableros</a>
                        {mostrarLogin ? (
                            <a href="/login" className="py-2 text-center">Login</a>
                        ) : null}
                    </div>
                </div>
            </div>
        </nav>
    );
}
