"use client"

import { useEffect, useState } from "react";

export default function Navbar() {
    const [mostrarLogin, setMostrarLogin] = useState(true);

    useEffect(() => {
        // Verificar si el usuario está autenticado
        // var user = null;
        // if (user) {
        //     setMostrarLogin(false);
        // }
    }, []);

    return (
            <nav className="flex justify-between items-center p-4">
                <h1 className="font-bold text-4xl">Simpled.</h1>

                <div className={`grid ${mostrarLogin ? 'grid-cols-4' : 'grid-cols-3'} divide-x`}>
                    <a href="/" className="text-lg px-4 text-center">Inicio</a>
                    <a href="/about" className="text-lg px-4 text-center">Nosotros</a>
                    <a href="/contact" className="text-lg px-4 text-center">Tableros</a>
                    {mostrarLogin ? (
                        <a href="/login" className="text-lg px-4 text-center">Login</a>
                    ) : null}
                </div>


            </nav>
    );
}
