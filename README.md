## Proyecto Backend
Backend en .NET para la gestión de trámites y horarios de la Universidad (UEES). Proporciona autenticación (JWT), gestión de usuarios/estudiantes, carreras y trámites, control de fechas/horarios disponibles y envío de correos electrónicos; está diseñado como una aplicación web API con arquitectura en capas (Domain / Application / Infrastructure).

### Stack
- Lenguaje: C# (.NET)
- Framework / runtime: ASP.NET Core (Web API) + Entity Framework Core (persistencia)
- Dependencias relevantes detectadas: Entity Framework Core (ApplicationDbContext, Migrations), sistema de identidad/seguridad (carpeta Identity), mapeos (carpeta Mapping — p. ej. AutoMapper o similar), y librería de envío de correo (implementada en EmailService).

## Cómo está organizado
Estructura relevante (carpetas / proyectos dentro de UeesDigital):
```
UeesDigital/
  UeesDigital.slnx                     solución .NET
  UeesDigital/                         (probablemente proyecto Web/API - capa de presentación)
  UeesDigital.Application/             servicios de aplicación; lógica de casos de uso
    Services/                          AuthService.cs, CarreraService.cs, EmailService.cs, FechaDisponibleService.cs, HorarioDisponibleService.cs, TramiteService.cs
    UeesDigital.Application.csproj
  UeesDigital.Domain/                  entidades e interfaces de dominio
    Entities/                          BaseEntity.cs, Carrera.cs, Estudiante.cs, Facultad.cs, FechaDisponible.cs, HorarioDisponible.cs, Tramite.cs
    Interfaces/                        repositorios e interfaces: IBaseRepository.cs, ICarreraRepository.cs, IEstudianteRepository.cs, ITramiteRepository.cs, IJwtService.cs, IEmailService.cs, ...
    UeesDigital.Domain.csproj
  UeesDigital.Infrastructure/          implementación técnica
    Persistence/
      ApplicationDbContext.cs
      Repositories/                     CarreraRepository.cs, EstudianteRepository.cs, FechaDisponibleRepository.cs, HorarioDisponibleRepository.cs, TramiteRepository.cs, UserRepository.cs
    Identity/                           (implementaciones de identidad)
    Mapping/                            perfiles de mapeo DTO↔Entidades
    Migrations/                         migraciones EF Core
    UeesDigital.Infrastructure.csproj
  .gitignore, .vs, obj/                 archivos de solución/IDE
```

Cómo encaja (flujo): la solución sigue un patrón tipo Clean Architecture / por capas:
- Domain: modelos y contratos (interfaces de repositorio y servicios).
- Application: orquesta casos de uso y contiene servicios que aplican la lógica del dominio (AuthService, TramiteService, EmailService, etc.).
- Infrastructure: implementa repositorios con EF Core (ApplicationDbContext + Repositories), seguridad/identity y mapeos; aquí están las migraciones y la integración con base de datos y SMTP u otros servicios externos.
- El proyecto de presentación (UeesDigital) expone la API HTTP usando los servicios de Application.

## Cómo ejecutarlo (mínimo para desarrollar / probar local)
Prerequisitos:
- .NET SDK (versión compatible con el proyecto; instalar .NET 6+ si no estás seguro).
- Base de datos compatible (SQL Server / PostgreSQL u otra según la cadena de conexión).
- Credenciales SMTP si quieres probar envío de correo.

Pasos básicos desde un clon:
1. Restaurar dependencias y compilar:
   - dotnet restore UeesDigital/UeesDigital.slnx
   - dotnet build UeesDigital/UeesDigital.slnx
2. Configurar variables de entorno / appsettings:
   - ConnectionStrings__DefaultConnection (cadena de conexión a la base de datos)
   - JWT__Secret (secreto para tokens JWT)
   - SMTP_HOST, SMTP_PORT, SMTP_USER, SMTP_PASS (o el esquema que use EmailService)
   - Cualquier otra configuración en appsettings.json del proyecto principal
   Ejemplo (.env):
   ```
   ConnectionStrings__DefaultConnection="Server=.;Database=UeesDigitalDb;Trusted_Connection=True;"
   JWT__Secret="TU_SECRETO_LARGO_AQUI"
   SMTP_HOST="smtp.ejemplo.com"
   SMTP_PORT=587
   SMTP_USER="usuario@ejemplo.com"
   SMTP_PASS="password"
   ```
3. Aplicar migraciones (desde la carpeta de Infrastructure si se usan herramientas EF Core):
   - dotnet ef database update --project UeesDigital/UeesDigital.Infrastructure --startup-project UeesDigital/UeesDigital
   (si no usas dotnet-ef, crea la base y ajusta la cadena de conexión)
4. Ejecutar la API:
   - dotnet run --project UeesDigital/UeesDigital
   O bien ejecutar desde el IDE (Visual Studio / Rider) abriendo UeesDigital.slnx y arrancando el proyecto de presentación.

Notas adicionales:
- Revisa UeesDigital.Infrastructure/Persistence/ApplicationDbContext.cs para ver modelos, DbSets y configuraciones de EF Core.
- Las migraciones están en UeesDigital.Infrastructure/Migrations.
- Los servicios de negocio están en UeesDigital.Application/Services (AuthService.cs, TramiteService.cs, EmailService.cs, FechaDisponibleService.cs, HorarioDisponibleService.cs, CarreraService.cs).

## Funcionalidades principales (resumido)
- Autenticación y autorización con JWT (AuthService / IJwtService).
- Gestión de usuarios (UserRepository).
- Gestión de carreras y estudiantes (CarreraService / EstudianteRepository).
- Gestión de trámites (TramiteService / TramiteRepository).
- Gestión de fechas y horarios disponibles (FechaDisponibleService, HorarioDisponibleService y sus repositorios).
- Envío de correos (EmailService / IEmailService).
- Persistencia con Entity Framework Core mediante ApplicationDbContext y repositorios concretos.

## Para contribuir
- Clona el repositorio y abre la solución UeesDigital.slnx en tu IDE .NET favorito.
- Crea una rama por feature/bugfix: feature/<descripcion>.
- Añade pruebas (si existen pruebas en el repo principal, sigue la convención).
- Asegúrate de actualizar/migrar la base de datos si cambias modelos.
