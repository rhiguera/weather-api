# WeatherApp — Avalonia Desktop (Linux) — Weather client

Lightweight, multiplatform desktop client built with Avalonia UI and organized using Clean Architecture + MVVM.

## Objetivo
- Aplicación de escritorio para Linux que consume una API del tiempo (OpenWeatherMap u otro).
- Arquitectura: Clean Architecture + MVVM.

## Estructura de la solución
- `WeatherApp.sln`
  - `WeatherApp.UI` (Avalonia views & viewmodels)
  - `WeatherApp.Application` (casos de uso)
  - `WeatherApp.Domain` (entidades e interfaces)
  - `WeatherApp.Infrastructure` (clientes HTTP, adaptadores)
  - `WeatherApp.Tests` (xUnit)

## Arquitectura y responsabilidades
- Domain: modelos puros (`Weather`, `Location`) y contratos (`IWeatherService`).
- Application: casos de uso (por ejemplo `GetWeatherUseCase`) que orquestan lógica de negocio.
- Infrastructure: implementaciones concretas (cliente HTTP, persistencia, adaptadores). Actualmente contiene un `WeatherServiceStub` para desarrollo.
- UI: Views (`.axaml`) y ViewModels desacoplados.

## Cómo usar (desarrollo)
1. Restaurar y compilar:

```bash
dotnet restore
dotnet build
```

2. Ejecutar la UI (Linux):

```bash
dotnet run --project WeatherApp.UI
```

3. En la fase inicial la app usa un stub que responde para ciudades de ejemplo (`London`, `Madrid`, `Sydney`).

## Configuración de API real
- No incluir claves en el repositorio. Para integrar OpenWeatherMap más adelante:
  - Añadir cliente HTTP en `WeatherApp.Infrastructure` que implemente `IWeatherService`.
  - Leer `API_KEY` desde variables de entorno o `appsettings.Development.json` (no comitear keys).
  - Registrar implementación real en DI reemplazando el stub.

## Tests
- Framework: `xUnit`.
- Se añaden tests de unidades en `WeatherApp.Tests` para los casos de uso y ViewModels.

## Estrategia incremental y commits
- Pequeños commits por responsabilidad (ver `untitled:plan-weatherAppPlan.prompt.md`).
- Mensajes sugeridos: `feat:`, `fix:`, `test:` siguiendo Conventional Commits.

## Siguientes pasos
1. Implementar cliente HTTP stub (hecho).
2. Añadir tests para `GetWeatherUseCase`.
3. Conectar ViewModel a use case y mostrar datos en UI.
4. Reemplazar stub por implementación real con `HttpClient` y `Polly`.
