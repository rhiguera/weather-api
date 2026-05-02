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

  ### OpenWeatherMap client & resilience

  - Set environment variable `OPENWEATHER_API_KEY` to enable the real OpenWeatherMap client. If not set, the app falls back to a local stub.
  - The HttpClient used for OpenWeatherMap is configured with Polly policies: retries (exponential backoff) and a timeout. Network errors and timeouts are surfaced to the UI as friendly error messages.

  Example (Linux):

  ```bash
  export OPENWEATHER_API_KEY="your_api_key_here"
  dotnet run --project WeatherApp.UI
  ```

## Tests
- Framework: `xUnit`.
- Se añaden tests de unidades en `WeatherApp.Tests` para los casos de uso y ViewModels.

## Run instructions (quick)

Prerequisites:
- .NET 8 SDK
- Graphical session (X11/Wayland) for the UI on Linux

Build & run UI:

```bash
dotnet build
dotnet run --project WeatherApp.UI
# For verbose Avalonia logs (useful when diagnosing a blank window):
AVALONIA_LOG_LEVEL=debug dotnet run --project WeatherApp.UI
```

Optional: enable real OpenWeatherMap client (otherwise a stub is used):

```bash
export OPENWEATHER_API_KEY=your_api_key_here
dotnet run --project WeatherApp.UI
```

Run tests:

```bash
dotnet test
```

Troubleshooting:
- If the window appears blank, verify you're running in a desktop graphical session (not a headless SSH without X11/Wayland). Use `AVALONIA_LOG_LEVEL=debug` to capture runtime logs.
- Already tracked build artifacts were removed and `.gitignore` updated. To remove other tracked artifacts if present:

```bash
git rm -r --cached **/bin **/obj || true
git commit -m "chore: remove build artifacts from index"
```


## Estrategia incremental y commits
- Pequeños commits por responsabilidad (ver `untitled:plan-weatherAppPlan.prompt.md`).
- Mensajes sugeridos: `feat:`, `fix:`, `test:` siguiendo Conventional Commits.

## Siguientes pasos
1. Implementar cliente HTTP stub (hecho).
2. Añadir tests para `GetWeatherUseCase`.
3. Conectar ViewModel a use case y mostrar datos en UI.
4. Reemplazar stub por implementación real con `HttpClient` y `Polly`.
5. Mejoras UX implementadas: indicador de carga, deshabilitar controles durante peticiones, Enter para enviar.
