# Hostify

API para gestionar usuarios, alojamientos, reservas, reseñas y notificaciones.

## Antes de empezar

Necesitas tener instalado el SDK de .NET 8 y SQL Server Express. La configuración inicial del proyecto espera encontrar SQL Server en la instancia `SQLEXPRESS`.

## Preparar la configuración

1. Abre el proyecto `Hostify` y revisa su configuración local, `appsettings.Development.json`. Si no existe, créalo dentro de la carpeta `Hostify`.
2. Comprueba que la cadena `ConnectionStrings:DefaultConnection` apunta a tu instancia de SQL Server. Por defecto, la aplicación espera una base de datos llamada `HostifyDb`.
3. Configura una clave secreta para JWT. La aplicación la necesita para firmar los tokens de autenticación.
4. Configura el correo electrónico que utilizará la aplicación. El servicio de correo actual usa Gmail y necesita estos dos valores:
   - `Email:FromEmail`: la dirección de Gmail remitente.
   - `Email:AppPassword`: una contraseña de aplicación de Gmail.
5. Guarda la clave JWT y las credenciales del correo en los secretos de usuario de Visual Studio, en vez de escribirlas en archivos que puedan subirse al repositorio. Usa la sección `Email`; la configuración existente de `SendGrid` no corresponde al servicio de correo actual.

## Preparar la base de datos

Antes de iniciar la API, asegúrate de que SQL Server Express está instalado y en ejecución. Después, aplica a esa instancia las migraciones de Entity Framework incluidas en el proyecto. Esto crea la base de datos `HostifyDb` y las tablas necesarias. La API no crea la base de datos automáticamente al arrancar.

Si utilizas otra instancia o nombre de base de datos, actualiza `ConnectionStrings:DefaultConnection` para que coincida con tu configuración.

## Iniciar y probar la API

En Visual Studio, selecciona `Hostify` como proyecto de inicio y ejecuta la aplicación con el perfil de desarrollo. Swagger estará disponible en:

- `https://localhost:7131/swagger`
- `http://localhost:5056/swagger`

Desde Swagger puedes consultar y probar los endpoints disponibles. Para los endpoints protegidos, primero obtén un token JWT y úsalo como token Bearer.

## Importante

No publiques claves, contraseñas de aplicación ni otros secretos en el repositorio. Si el envío de correos no está configurado, las funciones que dependen de él no podrán enviar mensajes.