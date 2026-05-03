## Plan: Aplicación de Escritorio WeatherApp (Avalonia + Clean Architecture)

TL;DR - Crear una aplicación de escritorio multiplataforma (Linux) usando Avalonia UI, organizada con Clean Architecture + MVVM, consumiendo una API del tiempo (OpenWeatherMap). Desarrollaremos incrementalmente con commits pequeños y tests automatizados.

**Pasos**
1. Configuración inicial del repositorio: crear solución y proyectos base. *depends on nada*
2. Configurar proyecto UI base (Avalonia) y shell de MVVM. *depends on 1*
3. Crear capas Domain y Application (entidades, interfaces, casos de uso). *parallel with 2*
4. Implementar Infrastructure: cliente HTTP para la API (interface y stub/mock). *depends on 3*
5. Primer caso de uso: obtener el clima por ciudad (mockeado). Añadir tests unitarios para el caso de uso. *depends on 3,4*
6. UI mínima: vista para ingresar ciudad y mostrar resultado con ViewModel ligado a caso de uso (usando mock). *depends on 2,5*
7. Integración real con OpenWeatherMap: configuración de API key, HttpClient real, manejo de errores y retries (Polly). *depends on 4,6*
8. Tests de integración/infrastructure (mock HttpMessageHandler / MockHttp). *parallel with 7*
9. Mejoras UX/errores/formatos y empaquetado para Linux (AppImage/Flatpak). *depends on 7,8*

**Relevant files**
- WeatherApp.sln — solución contenedora
- WeatherApp.UI — proyecto Avalonia (Views, ViewModels)
- WeatherApp.Application — casos de uso, DTOs de uso interno
- WeatherApp.Domain — entidades, interfaces (IWeatherService, modelos core)
- WeatherApp.Infrastructure — implementación HttpClient, configuración, mappers
- WeatherApp.Tests — xUnit tests para Application e Infrastructure

**Verification**
1. `dotnet build` across solution (verify compile)
2. Unit tests run: `dotnet test` (xUnit)
3. Manual run on Linux: ejecutar binario Avalonia y comprobar petición mock y real
4. Automated CI: simple YAML que ejecute `dotnet restore`, `dotnet build`, `dotnet test`

**Decisions / Assumptions**
- Target .NET: preferible .NET 8 (o 7 si hay restricción). Ajustable si indicas otra.
- MVVM toolkit: usar CommunityToolkit.Mvvm (ligero) para `ObservableObject`, `RelayCommand`. Alternativa: ReactiveUI si quieres reactividad avanzada.
- JSON: `System.Text.Json` para deserialización.
- HttpClient: usar `IHttpClientFactory` y `Microsoft.Extensions.DependencyInjection` para DI y testabilidad.
- Retries/circuit-breaker: usar `Polly` para robustez de red (opcional en MVP).
- Mocking: usar `Moq` o `RichardSzalay.MockHttp` para simular `HttpMessageHandler` en tests de integration infra.

**Further considerations**
1. Autenticación/config: almacenar API key en variables de entorno o archivo `appsettings.Development.json` (no commitear keys).
2. Internacionalización: planear formatos (Celsius/°F) y localización si es necesario más adelante.
3. Empaquetado: AppImage o Flatpak para Linux, o publicar como self-contained.

---

**Detalles por sección**

**1. Elección tecnológica**
- Avalonia UI: multiplataforma real (Windows/macOS/Linux), XAML-like, buena comunidad para apps desktop en .NET. Soporta Linux nativamente y se integra bien con MVVM.
- MAUI: orientado principalmente a mobile + escritorio, soporte Linux es limitado y depende de proyectos comunitarios; mayor foco en iOS/Android/Windows/Mac.
- WPF: muy maduro, pero Windows-only; no es viable si la prioridad es Linux.

Conclusión: Avalonia es la opción más directa y estable para una app desktop .NET en Linux.

**2. Arquitectura (Clean Architecture + MVVM)**
- Domain: entidades (Weather, Location), interfaces (IWeatherRepository / IWeatherService). Sin dependencias externas.
- Application: casos de uso (GetWeatherByCity), DTOs de aplicación, interfaces para ports (IWeatherServicePort). Contiene la lógica orquestadora y reglas de negocio mínimas.
- Infrastructure: implementación de los ports — cliente HTTP que llama a OpenWeatherMap, mapeadores DTO→Domain, configuración (API key). Contendrá clases concretas y adaptadores.
- UI (Avalonia): Views (.xaml) y ViewModels. Los ViewModels inyectan casos de uso de Application y exponen propiedades/commands para binding.

Responsabilidades claras:
- Domain: reglas y modelos puros.
- Application: coordenación y casos de uso testables.
- Infrastructure: detalles de E/S y adaptadores.
- UI: presentación y binding, sin lógica de negocio.

**3. Estructura de la solución**
- WeatherApp.sln
  - WeatherApp.UI (Avalonia)
  - WeatherApp.Application
  - WeatherApp.Domain
  - WeatherApp.Infrastructure
  - WeatherApp.Tests

Cada proyecto con un objetivo único y dependencias sólo hacia capas inferiores.

**4. MVVM en Avalonia**
- Organización:
  - `Views/` — XAML files (MainWindow.axaml, WeatherView.axaml)
  - `ViewModels/` — MainWindowViewModel, WeatherViewModel
- Binding:
  - Bindings declarativos en XAML hacia propiedades públicas en ViewModel con INotifyPropertyChanged (o CommunityToolkit.ObservableObject).
- Commands:
  - `ICommand` implementado vía `RelayCommand`/`AsyncRelayCommand`.
- Evitar code-behind salvo inicialización visual mínima. Toda lógica en ViewModels.

**5. Cliente API del tiempo**
- Usar `HttpClient` a través de `IHttpClientFactory` (registrado en DI).
- Configuración de API key por `IConfiguration` (appsettings + env vars).
- DTOs de la respuesta mapeados a entidades de `Domain` mediante mappers en Infrastructure.
- Manejo de errores: normalizar respuestas de error, retries con `Polly`, transformar excepciones en tipos de resultado para la UI.

**6. Testing**
- Framework: `xUnit`.
- Mocking: `Moq` para interfaces, `RichardSzalay.MockHttp` para HttpClient.
- Qué testear:
  - Application: casos de uso (flujos happy path y errores), mockeando `IWeatherRepository`.
  - Infrastructure: mapeo DTO→Domain y lógica de parsing (usar respuestas HTTP mockeadas).
  - UI: pruebas unitarias de ViewModels (sin Views) para comandos y estados.

**7. Estrategia incremental + commits**
Pequeños pasos con mensajes sugeridos (Conventional Commits):
1. Inicializar solución y proyectos
   - feat: init solution WeatherApp.sln and projects
2. Configurar Avalonia shell
   - feat(ui): add Avalonia app shell and MainWindow
3. Añadir Domain y Application skeleton
   - feat(domain): add core entities and interfaces
4. Implementar IWeatherClient stub + tests
   - feat(infra): add weather api client stub
   - test: add unit tests for GetWeather use case (mocked)
5. Conectar ViewModel con use-case (mock)
   - feat(ui): bind WeatherViewModel to GetWeather use-case (mock)
6. Reemplazar stub por HttpClient real + config
   - feat(infra): implement OpenWeatherMap client
7. Añadir tests de infraestructura y CI
   - test(ci): add tests and CI pipeline

Cada commit debe implementar y probar una única responsabilidad.

**8. MVP**
- Input: campo para introducir nombre de ciudad
- Output: temperatura actual y descripción (p. ej. "clear sky"), icono opcional
- Manejo básico de errores: ciudad no encontrada, sin conexión, mostrar mensajes de usuario
- Tests: caso de uso de obtención de clima y ViewModel

---

Si te parece bien este plan, lo guardaré en la memoria de sesión y luego puedo proceder a ejecutar el Paso 1 (crear la solución y los proyectos base). Si confirmas, empezaré con: "Crear solución y proyectos base" y prepararé el primer commit sugerido.