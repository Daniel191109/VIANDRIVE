# Configuración requerida en la nube

El archivo `appsettings.json` se dejó sin secretos. Antes de iniciar la aplicación, configura estas variables de entorno en el panel del proveedor (o mediante el gestor de secretos):

- `ConnectionStrings__DefaultConnection`: cadena de conexión SQL Server.
- `Jwt__Key`: secreto aleatorio de al menos 32 bytes (recomendado 64 bytes o más).
- `Jwt__Issuer`: debe coincidir con `Jwt:Issuer` (por defecto `viandrive`).
- `Jwt__Audience`: debe coincidir con `Jwt:Audience` (por defecto `viandrive-clientes`).
- `ElasticEmail__ApiKey` y `ElasticEmail__From`: para el correo de confirmación del registro.
- `Gmail__Correo` y `Gmail__ClaveApp`: para `POST /api/EmailApi/enviar-token`, si se conserva SMTP de Gmail.
- `Firebase__CredentialsPath`: ruta privada del archivo de credenciales de Firebase (opcional; sin él se desactivan las notificaciones push, pero la API no debe caerse).

No publiques estas variables ni las agregues al repositorio. Rota las credenciales que estuvieron en el ZIP original: base de datos, correo/SMTP, proveedor de correo y cualquier clave JWT o API que haya sido reutilizada.

## Endpoints de clientes

- `POST /api/ClienteApi/registrar` (compatibilidad)
- `POST /api/clientes/registrar` (alias REST)
- `POST /api/ClienteApi/login`
- `POST /api/clientes/login`

El JSON de registro usa `nombreCompleto`, `correo`, `celular`, `clave`; `metodoPagoPreferido` es opcional y usa `Efectivo` por defecto. La contraseña debe tener 8 caracteres o más, letras, números y un carácter especial.

Ejemplo de registro:

```json
{
  "nombreCompleto": "Cliente de prueba",
  "correo": "cliente.prueba@example.com",
  "celular": "3001234567",
  "clave": "PruebaSegura2026!",
  "metodoPagoPreferido": "Efectivo"
}
```

## Crear una carrera

- `POST /api/solicitudes` recibe `clienteId`, `origen`, `destino`, coordenadas, `ofertaPasajero` y opcionalmente `medioPago`. Si no se envía una oferta positiva, aplica el valor base 8000 que ya utilizaba el repositorio anterior.
- `POST /api/solicitudes/crear-basica` conserva la forma anterior con `usuarioId` y `tarifa`, pero ahora valida que ese ID exista en `Clientes`.

El ID debe ser el `clienteId` devuelto por el login del cliente. Ya no se sustituye silenciosamente por el ID 1.

Ejemplo para `POST /api/solicitudes`:

```json
{
  "clienteId": 7,
  "origen": "Parque Nariño, Pasto",
  "destino": "Aeropuerto Antonio Nariño",
  "latOrigen": 1.2136,
  "lngOrigen": -77.2811,
  "latDestino": 1.3969,
  "lngDestino": -77.2915,
  "ofertaPasajero": 18000,
  "medioPago": "Efectivo"
}
```

La API comprueba que `clienteId` exista en `Clientes`, inserta la solicitud y devuelve el `id` generado junto con el estado `Pendiente`.

## Aceptar una carrera

La ruta `PUT /api/solicitudes/{id}/aceptar` ahora requiere `conductorId` como query string, por ejemplo `?conductorId=12&lat=1.21&lng=-77.28`. La ruta `POST /api/solicitudes/contraoferta` conserva la opción de aceptar enviando `solicitudId`, `conductorId`, `esContraoferta=false`.

## Importante: revisar las claves foráneas existentes

El modelo corregido vincula `SolicitudesServicio.ClienteId` con `Clientes.Id` y `ConductorId` con `Conductores.Id`. Si la base de datos conserva una clave foránea antigua hacia `Usuarios`, revisa y ejecuta el script `Database/20261110_AlinearRelacionesSolicitudes.sql` después de hacer un respaldo. El script no elimina solicitudes existentes; deja las relaciones como no confiables para filas históricas y valida las nuevas escrituras.

La compilación y prueba contra la base de datos real deben ejecutarse en un entorno con .NET 9 y acceso a SQL Server. Este entorno de revisión no tiene instalado el SDK de .NET, por lo que no fue posible ejecutar `dotnet build` ni probar endpoints contra tu servidor.
