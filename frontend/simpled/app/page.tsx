import Banner from "@/components/banner";
import Image from "next/image";

export default function Home() {
  return (
    <>
      <div>
        <Banner>
          <h1 className="text-4xl font-bold mb-4">La herramienta definitiva para la gestión colaborativa de proyectos</h1>
          <p className="text-lg mb-8">Organiza tareas, proyectos e ideas de manera sencilla y en tiempo real, con todo lo que necesitas para trabajar en equipo.</p>
          <a href="#caracteristicas" className="bg-background text-foreground py-2 px-6 rounded-full text-xl hover:bg-foreground hover:text-background">Descubre más</a>
        </Banner>

        <section id="caracteristicas" className="py-16 px-4">
          <h2 className="text-3xl font-semibold text-center mb-8">¿Qué puedes hacer con Simpled?</h2>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-12">

            <div className="text-center">
              <Image src="/imagen.png" alt="Tablero de ejemplo" width={600} height={300} className="w-full h-48 object-cover rounded-lg mb-6" />
              <h3 className="text-2xl font-medium mb-4">Organiza tu trabajo en Tableros</h3>
              <p>Crea tableros personalizados para dividir tus proyectos y hacer un seguimiento claro de las tareas.</p>
            </div>

            <div className="text-center">
              <Image src="/imagen.png" alt="Colaboración en tiempo real" width={600} height={300} className="w-full h-48 object-cover rounded-lg mb-6" />
              <h3 className="text-2xl font-medium mb-4">Colabora en Tiempo Real</h3>
              <p>Trabaja simultáneamente con tu equipo, con cambios reflejados al instante gracias a la edición colaborativa.</p>
            </div>

            <div className="text-center">
              <Image src="/imagen.png" alt="Vista Kanban y Calendario" width={600} height={300} className="w-full h-48 object-cover rounded-lg mb-6" />
              <h3 className="text-2xl font-medium mb-4">Gestión Avanzada</h3>
              <p>Utiliza vistas tipo Kanban y calendarios para organizar tus tareas de manera visual y eficiente.</p>
            </div>
          </div>
        </section>

        <div className="py-8 px-4">
        <hr/>
        </div>
        
        <section className="py-16 px-4">
          <h2 className="text-3xl font-semibold text-center mb-8">Manejo de Roles y Permisos</h2>
          <div className="text-center max-w-4xl mx-auto">
            <Image src="/imagen.png" alt="Manejo de Roles y Permisos" width={1200} height={600} className="w-full h-64 object-cover rounded-lg mb-6" />
            <p className="text-lg mb-6">Asigna roles y permisos a tu equipo para controlar el acceso y la colaboración dentro de los tableros. De esta forma, puedes gestionar de manera estructurada quién puede ver, editar o administrar tareas.</p>
          </div>
        </section>

        <Banner>
          <h2 className="text-3xl font-semibold mb-4">¡Comienza a organizar tus proyectos hoy mismo!</h2>
          <p className="text-lg mb-8">Regístrate ahora y prueba todas las funciones de <b>Simpled</b>. ¡Es completamente gratis!</p>
          <a href="/registro" className="bg-background text-foreground py-2 px-6 rounded-full text-xl hover:bg-foreground hover:text-background">Regístrate</a>
        </Banner>
      </div>
    </>
  );
}
