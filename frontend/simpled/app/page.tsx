import Banner from "@/components/banner";
import Image from "next/image";
import FeatureCard from "@/components/FeatureCard";
import { Button } from "@/components/ui/button";
import {
  ArrowRight,
  Calendar,
  CheckCircle2,
  Layers,
  Users,
} from "lucide-react";

export default function Home() {
  return (
    <>
      <main className="flex flex-col min-h-screen">
        <Banner>
          <h1 className="text-4xl md:text-5xl font-bold mb-6">
            La herramienta definitiva para la gestión colaborativa de proyectos
          </h1>
          <p className="text-lg md:text-xl mb-8 max-w-3xl mx-auto">
            Organiza tareas, proyectos e ideas de manera sencilla y en tiempo
            real, con todo lo que necesitas para trabajar en equipo.
          </p>
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Button
              asChild
              size="lg"
              className="bg-white text-indigo-600 hover:bg-white/90"
            >
              <a href="#caracteristicas">Descubre más</a>
            </Button>
            <Button
              asChild
              size="lg"
              className="bg-white text-indigo-600 hover:bg-white/90"
            >
              <a href="/registro">Comenzar gratis</a>
            </Button>
          </div>
        </Banner>

        <section id="caracteristicas" className="py-20 px-4 bg-background">
          <div className="container mx-auto max-w-6xl">
            <h2 className="text-3xl md:text-4xl font-bold text-center mb-4">
              ¿Qué puedes hacer con Simpled?
            </h2>
            <p className="text-xl text-muted-foreground text-center mb-16 max-w-3xl mx-auto">
              Una plataforma completa para gestionar tus proyectos de forma
              eficiente y colaborativa
            </p>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
              <FeatureCard
                icon={<Layers className="h-10 w-10 text-indigo-500" />}
                title="Organiza tu trabajo en Tableros"
                description="Crea tableros personalizados para dividir tus proyectos y hacer un seguimiento claro de las tareas."
                imageUrl="/imagen.png"
              />

              <FeatureCard
                icon={<Users className="h-10 w-10 text-indigo-500" />}
                title="Colabora en Tiempo Real"
                description="Trabaja simultáneamente con tu equipo, con cambios reflejados al instante gracias a la edición colaborativa."
                imageUrl="/imagen.png"
              />

              <FeatureCard
                icon={<Calendar className="h-10 w-10 text-indigo-500" />}
                title="Gestión Avanzada"
                description="Utiliza vistas tipo Kanban y calendarios para organizar tus tareas de manera visual y eficiente."
                imageUrl="/imagen.png"
              />
            </div>
          </div>
        </section>

        <div className="py-8 px-4">
          <hr />
        </div>

        <section className="py-20 px-4 bg-muted/30">
          <div className="container mx-auto max-w-6xl">
            <div className="grid md:grid-cols-2 gap-12 items-center">
              <div>
                <h2 className="text-3xl font-bold mb-6">
                  Manejo de Roles y Permisos
                </h2>
                <p className="text-lg mb-6">
                  Asigna roles y permisos a tu equipo para controlar el acceso y
                  la colaboración dentro de los tableros. De esta forma, puedes
                  gestionar de manera estructurada quién puede ver, editar o
                  administrar tareas.
                </p>
                <ul className="space-y-3 mb-8">
                  <li className="flex items-start">
                    <CheckCircle2 className="h-6 w-6 text-indigo-500 mr-2 shrink-0 mt-0.5" />
                    <span>
                      Administradores con control total sobre el tablero
                    </span>
                  </li>
                  <li className="flex items-start">
                    <CheckCircle2 className="h-6 w-6 text-indigo-500 mr-2 shrink-0 mt-0.5" />
                    <span>Editores que pueden modificar tareas y columnas</span>
                  </li>
                  <li className="flex items-start">
                    <CheckCircle2 className="h-6 w-6 text-indigo-500 mr-2 shrink-0 mt-0.5" />
                    <span>Observadores con acceso de solo lectura</span>
                  </li>
                </ul>
              </div>
              <div className="rounded-xl overflow-hidden shadow-lg">
                <Image
                  src="/imagen.png"
                  alt="Manejo de Roles y Permisos"
                  width={600}
                  height={400}
                  className="w-full h-auto object-cover"
                />
              </div>
            </div>
          </div>
        </section>

        <Banner>
          <h2 className="text-3xl font-bold mb-4">
            ¡Comienza a organizar tus proyectos hoy mismo!
          </h2>
          <p className="text-xl mb-8">
            Regístrate ahora y prueba todas las funciones de <b>Simpled</b>. ¡Es
            completamente gratis!
          </p>
          <Button
            asChild
            size="lg"
            className="bg-white text-indigo-600 hover:bg-white/90"
          >
            <a href="/registro">Regístrate</a>
          </Button>
        </Banner>
      </main>
    </>
  );
}