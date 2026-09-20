# RentaFácil S.A.S.

Sistema básico de gestión de reservas de vehículos con microservicios en .NET, Angular, SQL Server y Azure-ready deployment.

## 1. Objetivo

Este proyecto implementa una solución para:
- Registrar vehículos
- Consultar disponibilidad por tipo y rango de fechas
- Asociar un cliente a una reserva
- Consultar historial de reservas por cliente
- Procesar reportes diarios con un worker

## 2. Arquitectura

```text
+-----------------+        +-------------------+        +---------------------+
| Angular Frontend| <-->  | BookingService    | <-->  | VehicleService      |
| localhost:4200  |        | CRUD reservas     |        | Disponibilidad      |
+-----------------+        +-------------------+        +---------------------+
           |                           |                            |
           |                           |                            |
           v                           v                            v
      SQL Server (BookingDb)       SQL Server (VehicleDb)       SQL Server (ReportDb)
                                                                  |
                                                                  v
                                                          ReportWorker
```

## 3. Stack tecnológico

- Backend: .NET 10 / ASP.NET Core / EF Core
- Microservicios: VehicleService, BookingService, ReportWorker
- Base de datos: SQL Server
- Frontend: Angular 18
- Docker Compose: SQL Server local + servicios .NET
- Azure: AKS, Azure SQL, Azure DevOps pipelines (diseño/documentado)

## 4. Principios aplicados

- SOLID
- DTO para comunicación entre capas
- Clean Architecture / separación de responsabilidades
- Repository pattern
- Dependency Injection
- Logging simple con consola
- Validaciones de negocio en Application Services
- Swagger para documentación de APIs
- Uso de microservicios con integración HTTP entre servicios

## 5. Estructura del repositorio

```text
src/
  VehicleService/
  BookingService/
  ReportWorker/
frontend/
  src/
  package.json

tests/
  Rentafacil.Tests/

docker-compose.yml
README.md
.gitignore
```

## 6. Instalación local

### Requisitos

- .NET 10 SDK
- SQL Server 2022 (local o Docker)
- Node 20+
- Angular CLI 18+

### 6.1 Configuración de base de datos

#### Opción 1: Usar Docker Compose (recomendado)

```bash
docker compose up -d sqlserver
```

Esto levanta SQL Server en `localhost:1433` con las credenciales:
- Usuario: `sa`
- Contraseña: `Passw0rd12345!`

#### Opción 2: SQL Server local instalado

Si tienes SQL Server instalado localmente, verifica que esté escuchando en puerto `1433` y con las mismas credenciales anteriores.

### 6.2 Variables de entorno y configuración

Los servicios utilizan SQL Server local por defecto (sin base de datos en memoria). Las connection strings se configuran en `appsettings.json`:

**BookingService:**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=RentaFacilBookingDb;User Id=sa;Password=Passw0rd12345!;TrustServerCertificate=True;Encrypt=False;"
}
```

**VehicleService:**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=RentaFacilVehicleDb;User Id=sa;Password=Passw0rd12345!;TrustServerCertificate=True;Encrypt=False;"
}
```

**ReportWorker:**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=RentaFacilReportDb;User Id=sa;Password=Passw0rd12345!;TrustServerCertificate=True;Encrypt=False;"
}
```

Las bases de datos se crean automáticamente al iniciar cada servicio (con `EnsureCreated()`).

### 6.3 Ejecutar backend

VehicleService:

```bash
dotnet run --urls http://localhost:5201 --project src/VehicleService/VehicleService.csproj
```

BookingService:

```bash
dotnet run --urls http://localhost:5202 --project src/BookingService/BookingService.csproj
```

ReportWorker:

```bash
dotnet run --project src/ReportWorker/ReportWorker.csproj
```

### 6.4 Ejecutar frontend

```bash
cd frontend
npm install
npx ng serve --host 0.0.0.0 --port 4200
```

Abrir: http://localhost:4200

## 7. API REST

### VehicleService

- POST /api/vehicles
- GET /api/vehicles/availability?vehicleType=Sedan&startDate=2026-09-10&endDate=2026-09-15
- POST /api/vehicles/reserve

### BookingService

- POST /api/bookings/clients
- POST /api/bookings
- GET /api/bookings/client/{clientId}

La documentación Swagger queda disponible en:
- http://localhost:5201/swagger
- http://localhost:5202/swagger

## 8. Pruebas y cobertura

Se añadieron pruebas unitarias para la lógica central de negocio:

```bash
dotnet test tests/Rentafacil.Tests/Rentafacil.Tests.csproj --collect:"XPlat Code Coverage"
```

Cobertura observada en el reporte generado:
- line-rate: 0.546
- equivalentes a 54.6% de cobertura de líneas

Esto supera el umbral solicitado de >10%.

## 9. Configuración de base de datos

El proyecto utiliza **SQL Server local** como base de datos principal en todos los servicios:

- **BookingService** → `RentaFacilBookingDb`
- **VehicleService** → `RentaFacilVehicleDb`
- **ReportWorker** → `RentaFacilReportDb`

### Base de datos en memoria (desarrollo alternativo)

Si deseas usar base de datos en memoria para pruebas rápidas, puedes cambiar en `appsettings.Development.json`:

```json
{
  "UseInMemoryDatabase": true
}
```

> ⚠️ Por defecto está establecido en `false` para usar SQL Server local.

## 10. Buenas prácticas y decisiones de diseño

- Separación por capas: Domain, Application, Infrastructure, Presentation
- DTOs para controlar contratos y evitar acoplamiento fuerte
- Validaciones en servicio de aplicación
- Repositorios para persistencia y consultas
- Logging simple con consola
- Tratamiento de errores con mensajes de negocio claros
- Microservicios con comunicación HTTP para desacoplar lógica de reserva y disponibilidad

## 11. Azure

Aunque no se ejecutó despliegue real en Azure por limitaciones de credenciales, la solución está preparada para ese patrón:

### 11.1 AKS / Contenedores

- Los microservicios se empaquetan con Dockerfiles
- Se pueden desplegar en AKS con Helm o manifestos YAML
- Se recomienda usar Azure Container Registry (ACR) como registro privado

### 11.2 Azure SQL Database

- Se recomienda mover SQL Server local a Azure SQL Database
- Configurar cadenas de conexión con Managed Identity o secretos de Azure Key Vault


## 12. Git y commits

Este repositorio se inicializa con historial de funcionamiento por funcionalidades en ramas/commits. La sugerencia es mantener commits por tema:

- scaffold backend
- implement vehicle service
- implement booking service
- implement worker
- implement frontend
- add docker and docs

## 13. Colección Postman (opcional)

También se recomienda importar una colección con estos endpoints:
- /api/vehicles/availability
- /api/vehicles
- /api/bookings/clients
- /api/bookings
- /api/bookings/client/{id}

## 14. Conclusión

El proyecto entrega una base para una solución de reservas de vehículos con arquitectura distribuida, buenas prácticas, soporte de SQL Server y despliegue listo para Azure.
