# Partes 5 y 6: cómo meterlo en tu repo

Todo esto es C#, así que se trabaja en **Rider**.

## 1. Copiar los archivos

Descomprime el zip **encima** de tu carpeta `DnpAssigment` (la que tiene el `.sln`) y acepta reemplazar.

Archivos nuevos:
- `Client/BlazorApp/...`: el proyecto Blazor entero (Parte 5 y 6).
- `Shared/ApiContracts/LoginRequest.cs`: DTO del login (Parte 6).
- `Server/WebApplication3/Controllers/AuthController.cs`: endpoint `POST /auth/login` (Parte 6).

Archivos modificados:
- `DnpAssigment.sln`: añade la carpeta `Client` con el proyecto `BlazorApp`.
- `UsersController.cs`: ahora no deja crear dos usuarios con el mismo username (si no, el login no sabría cuál es).

Tu `users.json` tiene tres usuarios `crist` repetidos. El login funciona igual (coge el primero), pero puedes borrar los repetidos.

## 2. Abrir en Rider

1. Abre `DnpAssigment.sln`. Debe aparecer la carpeta **Client** con **BlazorApp** dentro.
2. Si Rider no lo ve, haz clic derecho en la solución → *Reload*. Si sigue sin verlo: clic derecho en la solución → *Add → Existing Project* → `Client/BlazorApp/BlazorApp.csproj`.
3. *Build → Rebuild Solution*.

## 3. Ejecutar (siempre los dos a la vez)

1. Arranca **WebApplication3** con el perfil **https** (tiene que escuchar en `https://localhost:7018`).
2. Arranca **BlazorApp** (perfil https, se abre en `https://localhost:7257`).

Para lanzar los dos a la vez: *Run → Edit Configurations → + → Compound*, y añade los dos.

Si el Web API te sale en otro puerto, cambia la URL en `Client/BlazorApp/Program.cs` (`BaseAddress`).

Si sale un error de certificado HTTPS, ejecuta una vez en la terminal: `dotnet dev-certs https --trust`.

## 4. Qué cubre cada parte

**Parte 5 (requisitos mínimos)**
- Crear usuario → `/users/create` (Sign up)
- Crear post → `/posts/create`
- Lista de posts con título y autor → `/posts`
- Detalle del post (título, cuerpo, autor, comentarios) → clic en un post
- Añadir comentario → en el detalle del post
- Extra: lista de usuarios con filtro, filtrar posts por título y por usuario.
- Los servicios HTTP están en `Services/` con su interfaz (`IUserService` + `HttpUserService`, etc.), como pide el enunciado.

**Parte 6**
- Servidor: `AuthController` comprueba usuario y contraseña y devuelve un `UserDto` (sin contraseña).
- Cliente: `Auth/SimpleAuthProvider` convierte el `UserDto` en claims (`Name` e `Id`) y lo guarda en `sessionStorage` (la parte opcional), así no te desloguea al refrescar.
- `Routes.razor` envuelto en `CascadingAuthenticationState` + `AuthorizeRouteView`.
- Página `/login`, botón *Log out* en el menú.
- Autorización:
  - Solo logueados pueden crear posts y comentar (`AuthorizeView`).
  - El autor del post/comentario sale del usuario logueado, ya no está a fuego.
  - Solo el autor ve los botones de editar/borrar su post y borrar sus comentarios.

## 5. Diferencias con el enunciado (por si te preguntan)

- **Paquete NuGet `Microsoft.AspNetCore.Components.Authorization`**: no hace falta añadirlo, con .NET 10 ya viene incluido en el SDK web. Si tu profe quiere verlo, añádelo desde NuGet en BlazorApp con una versión 10.x.
- **Uso `AuthorizeView` en vez de `@attribute [Authorize]` en las páginas**: en un *Blazor Web App*, `[Authorize]` en una página hace que el servidor pida un middleware de autenticación que no tenemos y da error. `AuthorizeView` hace lo mismo sin ese problema.
- **`Routes.razor`**: el enunciado pone `AuthorizeRouteView` **y** `RouteView` a la vez, lo que pinta cada página dos veces. Aquí solo está `AuthorizeRouteView`.
- **Render mode `InteractiveServer` sin prerender** en `App.razor`, para que el `sessionStorage` (JavaScript) esté disponible desde el principio.

## 6. Subirlo

Haz commit y push desde Rider (*Git → Commit*). Comprueba que los `.csproj` y la carpeta `Client` entran en el commit.
