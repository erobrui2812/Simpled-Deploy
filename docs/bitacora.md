# 📘 Bitácora de Desarrollo - Proyecto TFG: Simpled

Registro de tareas realizadas durante el desarrollo del proyecto **Simpled**.

---

| Fecha       | Autor           | Tarea Realizada                                                                 |
|-------------|------------------|----------------------------------------------------------------------------------|
| 2025-03-18  | Adrián Jiménez   | Estructura inicial del backend con ASP.NET Core 8                               |
| 2025-03-19  | Elías Robles     | Creación de entidades: User, Board, Column, Item, Content                       |
| 2025-03-20  | Adrián Jiménez   | Configuración JWT para autenticación y protección de rutas                      |
| 2025-03-21  | Elías Robles     | Implementación de UserController + DTOs                                         |
| 2025-03-22  | Ambos            | Login con AuthController y roles con claims en JWT                              |
| 2025-03-23  | Adrián Jiménez   | CRUD completo para Items + Upload de ficheros                                   |
| 2025-03-23  | Elías Robles     | CRUD de Columns con validación de BoardId existente                             |
| 2025-03-24  | Ambos            | CRUD de Boards y validación de OwnerId                                          |
| 2025-03-24  | Adrián Jiménez   | Configuración de Swagger con autenticación Bearer                               |
| 2025-03-25  | Elías Robles     | Reestructuración en DTOs + protección de endpoints con [Authorize]              |
| 2025-03-25  | Ambos            | Preparación de anteproyecto en Notion y redacción del Readme.md                 |
| 2025-03-25  | Elías Robles     | Creación del esqueleto de front y algunas páginas de prueba                     |
| 2025-03-25  | Elías Robles     | Elección de fuentes y colores para la web + Creación de navbar                  |
| 2025-03-25  | Elías Robles     | Creación form login y registro + Agregar ShadCN + Toggle temas Claro/Oscuro     |
| 2025-03-28  | Adrián Jiménez   | Refactor completo a arquitectura por servicios + inyección de dependencias      |
| 2025-03-28  | Adrián Jiménez   | Implementación de excepciones personalizadas y middleware global                |
| 2025-03-28  | Adrián Jiménez   | CRUD completo para BoardMembers con soporte para inserción única o múltiple     |
| 2025-03-28  | Adrián Jiménez   | Aplicación de control de roles (`admin`, `editor`, `viewer`) en endpoints       |
| 2025-03-28  | Elías Robles     | Componentes navbar burguer, banner y footer con responsive para toda la web     |
| 2025-03-28  | Elías Robles     | Landing page responsive con componentes creados                                 |
| 2025-03-29  | Elías Robles     | Creación del context de auth, arreglos a algun textos                           |
| 2025-03-29  | Elías Robles     | Vincular los forms de login y registro con el context, cambios al layout        |
| 2025-03-29  | Adrián Jiménez   | Gestión completa de tableros por usuario con rol `admin` por defecto            |
| 2025-03-29  | Adrián Jiménez   | Filtrado de tableros públicos si no hay sesión, y propios si el usuario está logueado |
| 2025-03-29  | Adrián Jiménez   | Vista de tableros `/boards` terminada con botón editar/eliminar según propietario |
| 2025-03-29  | Adrián Jiménez   | Implementación de vista de detalle del tablero con columnas, items y miembros   |
| 2025-03-29  | Adrián Jiménez   | Correcciones en rutas dinámicas con `params.id` y errores con parse de tokens   |
| 2025-03-30  | Elías Robles     | Creacion de layout para un tablero individual                                   |
| 2025-03-30  | Elías Robles     | Maquetacion e intento de sidebar colapsable para la vista de tablero/id         |
| 2025-03-30  | Elías Robles     | Investigar como incluir TinyMCE para las tablas y los elementos                 |
| 2025-03-30  | Adrián Jiménez   | Implementación de creación de columnas e ítems desde la vista de tablero        |
| 2025-03-30  | Adrián Jiménez   | Integración de permisos personalizados para crear, editar y borrar ítems        |
| 2025-04-04  | Adrián Jiménez   | Implementación del sistema de invitaciones (crear, aceptar, rechazar) con SignalR y tokens |
| 2025-04-04  | Adrián Jiménez   | Creación del `SignalRContext` en el frontend y conexión global con eventos en tiempo real  |
| 2025-04-04  | Adrián Jiménez   | Corrección del filtrado de tableros por propiedad (`OwnerId`) y mejoras en visibilidad de boards |
| 2025-04-04  | Adrián Jiménez   | Verificación de toast en frontend, pruebas de eventos simultáneos en Swagger y conexión entre usuarios |
| 2025-04-04  | Elías Robles     | Mejoras visuales, mas componetizacion para reusabilidad de los componentes de link en el sidebar de tablero |
| 2025-04-05  | Elías Robles     | Mejoras visuales de la interfaz, mejoras visuales al Board y intento de Sidebar dinámico, aun queda cosas por implementar  |
| 2025-04-06  | Elías Robles     | Figma y inicio del sistema de gamificacion sin exito  |
| 2025-04-05  | Adrián Jiménez   | Vista "Sobre Nosotros" maquetada e implementada  |
| 2025-04-05  | Adrián Jiménez   | Figma: Paleta de colores, tipografia, componentes iniciales, vistas Landing y Registro/Login  |
| 2025-04-11  | Elías Robles     | Rutas restringidas en el front, reintento del sistema gamificación |
| 2025-04-13  | Elías Robles     | Continuacion sistema gamificación |
| 2025-04-20  | Elías Robles     | Continuacion sistema gamificación y preparacion de endpoints, controller y fix de lógica |
| 2025-04-21 | Adrián Jiménez    | Implementación completa de funcionalidad Drag & Drop para tareas entre columnas utilizando @dnd-kit/core |
| 2025-04-21  | Elías Robles     | Vista perfil, logica de authcontext para perfil y inicio vista logros |
| 2025-04-23  | Elías Robles     | Vista perfil tweaks, vista logros y modal de editar en front, endpoints de logros en back |
| 2025-04-23  | Elías Robles     | Instalación y ejecucion de prettier en el front |
| 2025-04-23  | Elías Robles     | Refactorización de variables, atributos y metodos de español a inglés |


---

📌 *Se continuará completando este fichero durante el resto del desarrollo y revisión previa a la entrega final.*
