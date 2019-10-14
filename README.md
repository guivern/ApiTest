# ApiTest
REST API desarrollado en C# con el framework .Net Core 2.2 que expone servicios para operaciones CRUD de Países y Ciudades sobre una base de datos SQLite.

## Requerimientos
Descargar e Instalar .Net Core SDK 2.2
* Linux: https://dotnet.microsoft.com/download/linux-package-manager/ubuntu18-04/sdk-2.2.402
* Windows: https://dotnet.microsoft.com/download/thank-you/dotnet-sdk-2.2.402-windows-x64-installer
## Instalación
Luego de descargar el proyecto, posicionarse en la carpeta raiz del proyecto desde la terminal y ejecutar los siguientes comandos:
1. `dotnet restore` para descargar las dependencias. 
2. `dotnet ef database update` crea una bd sqlite. 
3. `dotnet run` para iniciar la aplicación.
  * La aplicación corre en: http://localhost:5000
  * `Swagger UI`: http://localhost:5000/swagger
  
## Diseño de la API

| HTTP METHOD | POST            | GET       | PUT         | DELETE |
| ----------- | --------------- | --------- | ----------- | ------ |
| CRUD OP     | CREATE          | READ      | UPDATE      | DELETE |
| /paises       | Crea un país | Lista países | - | - |
| /paises/1  | -           | Retorna país si existe, sino retorna 404   | Actualliza país si existe, sino retorna 404 | Elimina país si existe y no posee ciudades, retorna 400 si existe país y tiene ciudades, retorna 400 si no existe país |
| /ciudades       | Crea una ciudad | Lista ciudades | - | - |
| /ciudades/1  | -           | Retorna ciudad si existe, sino retorna 404   | Actualliza ciudad si existe, sino retorna 404 | Elimina ciudad si existe, retorna 404 si no existe país |

**Obs**: Se pueden aplicar paginación, ordenamiento y filtro a las listas, a través de query params. Ej:

`/ciudades?orderBy=nombre:asc&filter=asuncion&pageSize=5&pageNumber=1`


## Autenticación
**Obs**: Todos los endpoints están protegidos por JWT, por lo que deberá autenticarse antes de probar los servicios.

Puede autenticarse enviando una petición GET a: http://localhost:5000/auth. El servidor responderá con un token el cual podrá copiar y pegar como cabecera de autorización para las demás peticiones.

## Ejemplos:
* Postman Collection: https://github.com/guivern/ApiTest/blob/master/ApiTest.postman_collection.json
* Para más ejemplos acceder a la interfaz de `swagger`  http://localhost:5000/swagger



