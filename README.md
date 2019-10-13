# ApiTest
REST API desarrollado en C# con el framework .Net Core 2.2 que expone servicios para operaciones CRUD de Países y Ciudades sobre una base de datos SQLite.

## Requerimientos
Descargar e Instalar .Net Core SDK 2.2
* Linux: https://dotnet.microsoft.com/download/linux-package-manager/ubuntu18-04/sdk-2.2.402
* Windows: https://dotnet.microsoft.com/download/thank-you/dotnet-sdk-2.2.402-windows-x64-installer
## Instalación
Luego de descargar el proyecto, posicionarse en la carpeta raiz desde la terminal y ejecutar los siguientes comandos:
1. `dotnet restore` para descargar las dependencias. 
2. `dotnet run` para iniciar la aplicación.
  * La aplicación corre en: http://localhost:5000
  * La interfaz del Swagger: http://localhost:5000/swagger

## Autenticación
**Obs**: Todos los endpoints están protegidos por JWT, por lo que deberá autenticarse antes de probar los servicios.

Puede autenticarse enviando una petición POST a: http://localhost:5000/auth con el siguiente json:

`{ "username":"invitado", "password":"password" }`

El servidor responderá con un token, el cual podrá copiar y pegar como cabecera de Authorization con el siguiente formato: `Bearer {token}` para las posteriores peticiones.

## Ejemplos
Luego de iniciar la aplicación y obtener el token de autorización, puede empezar a consumir la API con Postman o algun otro cliente de su preferencia. 




