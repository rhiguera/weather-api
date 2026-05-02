# WeatherApp — Avalonia Desktop (Linux) — Weather client

Lightweight, multiplatform desktop client built with Avalonia UI and organized using Clean Architecture + MVVM.

## Objetivo
- Aplicación de escritorio para Linux que consume una API del tiempo (OpenWeatherMap u otro).
- Arquitectura: Clean Architecture + MVVM.

## Arquitectura del Sistema

### Capas y Responsabilidades

```
┌─────────────────────────────────────────────┐
│          UI Layer (Avalonia)                │
│  ┌──────────────────────────────────────┐   │
│  │ MainWindow.xaml (View)               │   │
│  │ - Grid layout                        │   │
│  │ - Data bindings to ViewModel         │   │
│  └──────────────────────────────────────┘   │
│  ┌──────────────────────────────────────┐   │
│  │ MainWindowViewModel                  │   │
│  │ - INotifyPropertyChanged (MVVM Tk)   │   │
│  │ - Commands: GetWeatherCommand        │   │
│  │ - Properties: City, Temperature, etc │   │
│  └──────────────────────────────────────┘   │
└─────────────────────────────────────────────┘
           ↓ (Dependency Injection)
┌─────────────────────────────────────────────┐
│      Application Layer (Use Cases)          │
│  ┌──────────────────────────────────────┐   │
│  │ GetWeatherUseCase                    │   │
│  │ - Orchestrates business logic        │   │
│  │ - Calls IWeatherService             │   │
│  │ - Returns GetWeatherResponse        │   │
│  └──────────────────────────────────────┘   │
└─────────────────────────────────────────────┘
           ↓ (Interface: IWeatherService)
┌─────────────────────────────────────────────┐
│      Domain Layer (Business Rules)          │
│  ┌──────────────────────────────────────┐   │
│  │ Entities:                            │   │
│  │ - Weather (city, temp, description)  │   │
│  │ - Location (future)                  │   │
│  └──────────────────────────────────────┘   │
│  ┌──────────────────────────────────────┐   │
│  │ Interfaces:                          │   │
│  │ - IWeatherService (contract)         │   │
│  └──────────────────────────────────────┘   │
└─────────────────────────────────────────────┘
           ↓ (Dependency Injection)
┌─────────────────────────────────────────────┐
│    Infrastructure Layer (External Deps)    │
│  ┌──────────────────────────────────────┐   │
│  │ OpenWeatherMapService                │   │
│  │ - HttpClient (Polly resilience)      │   │
│  │ - Retry policy (exponential backoff) │   │
│  │ - Timeout handling (15s)             │   │
│  └──────────────────────────────────────┘   │
│  ┌──────────────────────────────────────┐   │
│  │ WeatherServiceStub (development)     │   │
│  │ - Mock data for testing (no network) │   │
│  └──────────────────────────────────────┘   │
│  ┌──────────────────────────────────────┐   │
│  │ DTOs & Converters                    │   │
│  │ - OpenWeatherMapWeatherDto           │   │
│  └──────────────────────────────────────┘   │
└─────────────────────────────────────────────┘
           ↓ (HTTP)
   ┌──────────────────────┐
   │ OpenWeatherMap API   │
   │ https://api.open...  │
   │ /data/2.5/weather?q=│
   └──────────────────────┘
```

### Estructura de Proyectos

```
WeatherApp.sln
├── WeatherApp.UI (Avalonia Desktop)
│   ├── App.axaml / App.axaml.cs
│   ├── MainWindow.axaml / MainWindow.axaml.cs
│   ├── Program.cs (entry point)
│   ├── ViewModels/
│   │   └── MainWindowViewModel.cs
│   └── Converters/
│       └── InverseBooleanConverter.cs
│
├── WeatherApp.Application (Use Cases)
│   └── UseCases/GetWeather/
│       ├── IGetWeatherUseCase.cs
│       ├── GetWeatherUseCase.cs
│       ├── GetWeatherRequest.cs
│       └── GetWeatherResponse.cs
│
├── WeatherApp.Domain (Business Logic & Entities)
│   ├── Entities/
│   │   ├── Weather.cs
│   │   └── Location.cs (future)
│   └── Interfaces/
│       └── IWeatherService.cs
│
├── WeatherApp.Infrastructure (External Dependencies)
│   ├── Services/
│   │   ├── OpenWeatherMapService.cs (real API)
│   │   └── WeatherServiceStub.cs (mock)
│   ├── Dto/
│   │   └── OpenWeatherMapWeatherDto.cs
│   └── DependencyInjection/
│       └── ServiceCollectionExtensions.cs
│
└── WeatherApp.Tests (xUnit)
    ├── GetWeatherUseCaseTests.cs
    ├── MainWindowViewModelTests.cs
    └── GlobalUsings.cs
```

### Flujo de Datos (Get Weather)

```
1. User enters city name in TextBox
   ↓
2. Presses Button or Enter
   ↓
3. MainWindowViewModel.GetWeatherCommand executes
   ↓
4. Calls GetWeatherUseCase.HandleAsync(request, cancellationToken)
   ↓
5. UseCase depends on IWeatherService (injected)
   ↓
6a. IF ENV OPENWEATHER_API_KEY set:
    → OpenWeatherMapService (real HTTP call)
    → Polly retries + timeout handling
    → JSON deserialization (DTO → Entity)
   
6b. ELSE:
    → WeatherServiceStub (mock data)
    → Hardcoded: London, Madrid, Sydney
   ↓
7. Response (Success or Error) returned to ViewModel
   ↓
8. ViewModel updates properties
   ↓
9. UI bindings refresh automatically
   ↓
10. Temperature, Description, or ErrorMessage displayed
```

### Configuración de Inyección de Dependencias

En `App.axaml.cs` se configura via `ServiceCollectionExtensions.AddInfrastructure()`:

```csharp
if (!string.IsNullOrWhiteSpace(apiKey)) {
    // Real API client with HttpClient + Polly
    services.AddHttpClient<IWeatherService, OpenWeatherMapService>(...)
} else {
    // Stub for development
    services.AddSingleton<IWeatherService, WeatherServiceStub>();
}
```

## Estructura de la solución
- `WeatherApp.sln`
  - `WeatherApp.UI` (Avalonia views & viewmodels)
  - `WeatherApp.Application` (casos de uso)
  - `WeatherApp.Domain` (entidades e interfaces)
  - `WeatherApp.Infrastructure` (clientes HTTP, adaptadores)
  - `WeatherApp.Tests` (xUnit)

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

## Tests
- Framework: `xUnit`.
- Se añaden tests de unidades en `WeatherApp.Tests` para los casos de uso y ViewModels.

## Guía de Ejecución

### Requisitos Previos
- .NET 8 SDK
- Sesión gráfica (X11/Wayland) para la UI en Linux

### Opción 1: Ejecución con Stub (por defecto - sin API key)

El stub contiene ciudades de ejemplo: `London`, `Madrid`, `Sydney`

```bash
dotnet build
dotnet run --project WeatherApp.UI
```

### Opción 2: Ejecución con API Real de OpenWeatherMap

**Paso 1:** Obtén una API key gratuita en https://openweathermap.org/api (Free tier - Current weather)

**Paso 2a (Recomendado):** Usa el script helper
```bash
./run-with-api.sh YOUR_API_KEY_HERE
```

**Paso 2b (Manual):** Configura variable de entorno
```bash
export OPENWEATHER_API_KEY="your_api_key_here"
dotnet build
dotnet run --project WeatherApp.UI
```

### Debugging

Para ver logs detallados de Avalonia (útil para diagnosticar problemas de renderización):
```bash
AVALONIA_LOG_LEVEL=debug dotnet run --project WeatherApp.UI
```

## Características de Resilencia

El cliente OpenWeatherMap está configurado con políticas Polly:
- **Retries:** hasta 3 intentos con backoff exponencial
- **Timeout:** 15 segundos por solicitud
- **Manejo de errores:** mensajes amigables en UI

Errores de red y timeouts se muestran como mensajes de error legibles para el usuario.
```
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
