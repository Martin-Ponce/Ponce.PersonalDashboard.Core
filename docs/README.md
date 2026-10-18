# Ponce.PersonalDashboard.Core

[Leer en español](#español)

## English

The backend of a personal-use app I made so I can add random useful things I'd like to keep track of in a single place. You can find the frontend here: Ponce.PersonalDashboard.Web (add repo link). Security is handled by my identity provider, which you can check out here: Ponce.IdentityProvider (add repo link).

This API also uses my template, which you can find here: Ponce.Templates.APITemplate (add repo link).

## The docs

These guides explain how the project is put together and how to add stuff to it. If it's your first time
around, go through them in order; after that just jump to whichever one you need.

| # | Guide | What's in it |
|---|-------|--------------|
| 1 | [Getting Started](01-getting-started.md) | How the projects are laid out, where settings come from (appsettings, user secrets, env vars), picking a database and running the API. |
| 2 | [Creating Entities](02-creating-entities.md) | The Domain side: entities, validators, error codes and length limits, repository contracts. |
| 3 | [Mapping Entities to the Database](03-mapping-entities-to-providers.md) | EF Core mappings for each database (SQL Server, Postgres, SQLite) and hooking a new `DbSet` into `AppDbContext`. |
| 4 | [Creating Endpoints](04-creating-endpoints.md) | DTOs and services, the minimal-API endpoints, and plugging a new feature into the app. |
| 5 | [Running in a Container](05-docker-container.md) | Building and running the Docker image, and passing in the connection string and other settings as env vars. |

## How it's organized

Four projects, each one a layer of a pretty simple Clean/Onion setup:

```
Ponce.PersonalDashboard.Core.Domain          <- entities, validators, repository contracts, exceptions
Ponce.PersonalDashboard.Core.Application      <- DTOs and services (this is where the business logic lives)
Ponce.PersonalDashboard.Core.Infrastructure   <- EF Core DbContext, mappings, repositories
Ponce.PersonalDashboard.Core.API              <- endpoints, DI and the general config
```

References only go inward: `API` → `Application`/`Infrastructure` → `Domain`, and `Domain` doesn't depend on
anything else.

This is the path a request takes:

```
HTTP request
  -> API/Endpoints/<Feature>/*.cs            (the endpoint)
  -> Application/Services/<Feature>Service.cs (the logic, calls the repository)
  -> Domain/Contracts/I<Feature>Repository.cs (the interface the service uses)
  -> Infrastructure/Repositories/<Feature>Repository.cs (the EF Core side)
  -> Infrastructure/Contexts/AppDbContext.cs  (the DbSet, mapped for whichever database you use)
```

## The example feature: `ExampleItem`

All the guides use the same small feature that's already in the code as their example: `ExampleItem`
(`Id`, `Name`, `Description`) with three endpoints (list, get by id and create). When a guide shows some
code, it comes straight from these files:

- Domain: [`Entities/Example/ExampleItem.cs`](../Ponce.PersonalDashboard.Core.Domain/Entities/Example/ExampleItem.cs), [`Validators/Entities/ExampleItemValidator.cs`](../Ponce.PersonalDashboard.Core.Domain/Validators/Entities/ExampleItemValidator.cs), [`Contracts/IExampleItemRepository.cs`](../Ponce.PersonalDashboard.Core.Domain/Contracts/IExampleItemRepository.cs)
- Application: [`Services/ExampleItemService.cs`](../Ponce.PersonalDashboard.Core.Application/Services/ExampleItemService.cs), `Dtos/Request/ExampleItems/*`, `Dtos/Response/ExampleItems/*`
- Infrastructure: [`Repositories/ExampleItemRepository.cs`](../Ponce.PersonalDashboard.Core.Infrastructure/Repositories/ExampleItemRepository.cs), `Mapping/SqlServer|Postgres|Sqlite/ExampleItem*Configuration.cs`
- API: `Endpoints/ExampleItems/*.cs`

## Adding a new feature, quick version

1. **[Create the entity](02-creating-entities.md)** in `Domain`: the entity, its validator, its length limits and the repository contract.
2. **[Map it to the database](03-mapping-entities-to-providers.md)** in `Infrastructure`: one `*Configuration` class per database, registered in the provider strategies, plus its `DbSet` in `AppDbContext`.
3. **[Build the endpoints](04-creating-endpoints.md)**: the DTOs and service in `Application`, the endpoint classes and group in `API`, then register the repository and service in DI and add the endpoint group to the routes.

Each guide goes through it step by step with the actual code.

---

## Español

El backend de una app de uso personal que hice para ir agregando cosas útiles de las que quiero llevar un
registro en un solo lugar. El frontend lo encuentras aquí: Ponce.PersonalDashboard.Web (add repo link). La seguridad la maneja
mi proveedor de identidad, que puedes ver aquí: Ponce.IdentityProvider (add repo link).

Esta API también usa mi plantilla, que está aquí: Ponce.Templates.APITemplate (add repo link).

### La documentación

Estas guías explican cómo está armado el proyecto y cómo agregarle cosas. Si es tu primera vez, léelas en
orden; después ya puedes ir directo a la que necesites. Las guías están en inglés.

| # | Guía | Qué tiene |
|---|------|-----------|
| 1 | [Getting Started](01-getting-started.md) | Cómo están organizados los proyectos, de dónde sale la configuración (appsettings, user secrets, variables de entorno), cómo elegir la base de datos y cómo correr la API. |
| 2 | [Creating Entities](02-creating-entities.md) | La parte de Domain: entidades, validadores, códigos de error y límites de longitud, y los contratos de los repositorios. |
| 3 | [Mapping Entities to the Database](03-mapping-entities-to-providers.md) | Los mapeos de EF Core para cada base de datos (SQL Server, Postgres, SQLite) y cómo agregar un `DbSet` nuevo a `AppDbContext`. |
| 4 | [Creating Endpoints](04-creating-endpoints.md) | Los DTOs y servicios, los endpoints de minimal API y cómo conectar una funcionalidad nueva a la app. |
| 5 | [Running in a Container](05-docker-container.md) | Cómo construir y correr la imagen de Docker, y cómo pasarle el connection string y demás configuración con variables de entorno. |

### Cómo está organizado

Son cuatro proyectos, cada uno es una capa de una arquitectura Clean/Onion bastante sencilla:

```
Ponce.PersonalDashboard.Core.Domain          <- entidades, validadores, contratos de repositorios, excepciones
Ponce.PersonalDashboard.Core.Application      <- DTOs y servicios (aquí vive la lógica de negocio)
Ponce.PersonalDashboard.Core.Infrastructure   <- DbContext de EF Core, mapeos, repositorios
Ponce.PersonalDashboard.Core.API              <- endpoints, inyección de dependencias y la configuración general
```

Las referencias solo van hacia adentro: `API` → `Application`/`Infrastructure` → `Domain`, y `Domain` no depende
de nada más.

Este es el camino que sigue una petición:

```
Petición HTTP
  -> API/Endpoints/<Feature>/*.cs            (el endpoint)
  -> Application/Services/<Feature>Service.cs (la lógica, llama al repositorio)
  -> Domain/Contracts/I<Feature>Repository.cs (la interfaz que usa el servicio)
  -> Infrastructure/Repositories/<Feature>Repository.cs (la parte de EF Core)
  -> Infrastructure/Contexts/AppDbContext.cs  (el DbSet, mapeado para la base de datos que uses)
```

### La funcionalidad de ejemplo: `ExampleItem`

Todas las guías usan como ejemplo la misma funcionalidad pequeña que ya está en el código: `ExampleItem`
(`Id`, `Name`, `Description`) con tres endpoints (listar, obtener por id y crear). Cuando una guía muestra
código, sale directo de estos archivos:

- Domain: [`Entities/Example/ExampleItem.cs`](../Ponce.PersonalDashboard.Core.Domain/Entities/Example/ExampleItem.cs), [`Validators/Entities/ExampleItemValidator.cs`](../Ponce.PersonalDashboard.Core.Domain/Validators/Entities/ExampleItemValidator.cs), [`Contracts/IExampleItemRepository.cs`](../Ponce.PersonalDashboard.Core.Domain/Contracts/IExampleItemRepository.cs)
- Application: [`Services/ExampleItemService.cs`](../Ponce.PersonalDashboard.Core.Application/Services/ExampleItemService.cs), `Dtos/Request/ExampleItems/*`, `Dtos/Response/ExampleItems/*`
- Infrastructure: [`Repositories/ExampleItemRepository.cs`](../Ponce.PersonalDashboard.Core.Infrastructure/Repositories/ExampleItemRepository.cs), `Mapping/SqlServer|Postgres|Sqlite/ExampleItem*Configuration.cs`
- API: `Endpoints/ExampleItems/*.cs`

### Agregar una funcionalidad nueva, versión rápida

1. **[Crea la entidad](02-creating-entities.md)** en `Domain`: la entidad, su validador, sus límites de longitud y el contrato del repositorio.
2. **[Mapéala a la base de datos](03-mapping-entities-to-providers.md)** en `Infrastructure`: una clase `*Configuration` por cada base de datos, registrada en las estrategias de cada proveedor, más su `DbSet` en `AppDbContext`.
3. **[Arma los endpoints](04-creating-endpoints.md)**: los DTOs y el servicio en `Application`, las clases de endpoints y su grupo en `API`, luego registra el repositorio y el servicio en la inyección de dependencias y agrega el grupo de endpoints a las rutas.

Cada guía lo explica paso a paso con el código real.
