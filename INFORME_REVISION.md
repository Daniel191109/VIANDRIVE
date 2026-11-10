# Informe de revisión técnica — VIANDRIVE

## Hallazgos confirmados en el ZIP recibido

1. `SolicitudServicioService.CrearSolicitudAsync` construía un objeto en memoria, pero la función auxiliar que debía persistirlo estaba vacía. El controlador respondía HTTP 200 como si la carrera ya estuviera guardada.
2. El endpoint alternativo `POST /api/solicitudes/crear-basica` usaba SQL directo, asignaba `ClienteId = 1` cuando no recibía un ID y escribía `0` como `ConductorId` y `VehiculoId`.
3. El alta e inicio de sesión de clientes operaban contra `Clientes`, mientras que `SolicitudServicio` y el repositorio consultaban `Usuarios` para el pasajero. También se consultaba `Usuarios` para obtener al conductor, aunque los conductores reales se guardaban en `Conductores`.
4. El token de cliente se firmaba con valores JWT hardcodeados diferentes del emisor/audiencia de `Program.cs`; por tanto, ese token no concordaba con la validación Bearer configurada.
5. Había operaciones de carrera (cancelar, contraofertar, rechazar, aceptar oferta, confirmar pago, finalizar y calificar) que devolvían éxito sin guardar modificaciones.
6. El servicio Firebase lanzaba una excepción al construir el controlador de vehículos si no estaba presente el archivo privado `firebase-key.json`.
7. Había credenciales privadas embebidas en archivos fuente y configuración.

## Cambios realizados

- El alta de clientes normaliza correo/celular, previene duplicados, mantiene el hash BCrypt y hace opcional el medio de pago (por defecto `Efectivo`).
- Se conservó la ruta `/api/ClienteApi/...` y se añadió `/api/clientes/...` como alias.
- El login firma tokens con `JwtService`, usando el mismo secreto, issuer y audience que valida el middleware.
- La creación principal y la ruta heredada `crear-basica` pasan por la misma lógica y guardan la solicitud mediante EF Core. El ID de cliente se valida contra `Clientes`; ya no se inventa un ID 1.
- Se alinearon las relaciones y las consultas de solicitudes con `Clientes`, `Conductores` y `Vehiculos`.
- Las operaciones de estado de carrera ahora cargan el registro, modifican las columnas correspondientes y ejecutan `SaveChangesAsync`; se eliminó el comportamiento de falso éxito.
- La ruta de aceptación requiere `conductorId`; la ruta de contraoferta asocia el conductor indicado.
- Firebase queda deshabilitado de forma controlada cuando faltan credenciales o no se puede inicializar, sin impedir que cargue el controlador de vehículos.
- Se retiraron secretos del código y `appsettings.json`; se documentaron las variables de entorno.
- Se incluyó un script SQL para alinear las claves foráneas, sujeto a revisión y respaldo previo.

## Límites de esta entrega

No se recibió el proyecto móvil, por lo que no pude comparar los nombres exactos de campos, URL base, ni payloads que la app realmente envía. Los ejemplos y rutas compatibles quedan documentados en `CONFIGURACION_NUBE.md`.

No hay SDK de .NET instalado en el entorno de análisis y no tuve acceso a tu SQL Server; por eso no puedo afirmar que el proyecto compile, que el script SQL corresponda exactamente a los nombres de constraints de tu servidor o que el despliegue ya haya quedado probado extremo a extremo. El script presupone SQL Server con las tablas `dbo.SolicitudesServicio`, `dbo.Clientes`, `dbo.Conductores` y `dbo.Vehiculos`, y columnas ID `int`.

Antes de publicar:

1. Rota las credenciales que venían en el archivo original.
2. Configura las variables de entorno de `CONFIGURACION_NUBE.md` en el proveedor de nube.
3. Haz un respaldo y verifica los datos históricos antes de ejecutar `Database/20261110_AlinearRelacionesSolicitudes.sql`.
4. Compila con .NET 9 y prueba registro, login, creación de carrera, listado de pendientes y aceptación con dos cuentas reales de prueba.
5. Valida que la app móvil mande el `clienteId` devuelto por el login y que use uno de los endpoints documentados.

Nota de seguridad: el proyecto todavía necesita una decisión de autorización de negocio. Varias rutas no tienen `[Authorize]` y el login de conductor no emite JWT. No he bloqueado esas rutas para no romper el contrato de la app móvil sin haber recibido su código; antes de producción debe definirse la autenticación de cliente y conductor y proteger las operaciones según rol.
