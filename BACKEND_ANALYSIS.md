# Backend Analysis Report: AppTransporte

This report provides a detailed analysis of the AppTransporte application's backend components, covering code quality, security, performance, and recommendations for improvement.

## 1. Backend Component Inventory

The backend logic is embedded within the .NET MAUI client application. The key components are:

-   **Service Layer**:
    -   `SqlServerService`: Central class for all database operations via stored procedures.
    -   `GoogleCloudStorageUploader`: Manages file uploads to Google Cloud Storage.
    -   `GoogleCloudAuthHelper`: Assists in managing credentials by copying them from the app package.

-   **Data Models (`/model`)**:
    -   Entities: `Cliente`, `Trabajador`, `Usuario`, `Persona`, `Solicitud`, `Pedido`, `Viaje`, `Vehiculo`, `Ubicacion`, `Seguimiento`, `Servicio`.
    -   Join/Link Entities: `ServicioPedido`, `TrabajadorViaje`.
    -   Lookup/Enum-like Entities: `Categoria`, `TipoDocumento`, `TipoUsuario`, `EstadoPedido`, `EstadoSolicitud`, `EstadoViaje`.
    -   Data Transfer Objects (DTOs): `UsuarioLogin`, `UsuarioResponse`.
    -   UI-specific model: `ServicioSeleccionable`.

-   **ViewModels (`/viewModel`)**:
    -   `BaseViewModel`: Base class providing `INotifyPropertyChanged` logic.
    -   `ClienteViewModel`, `VMTrabajadores`, `VMUbicacion`, `VMPedidos`, `VMViajes`, `VMmisSolicitudes`: Manage data and state for their respective views.
    -   `VMUsuario`, `VMEstado`: Placeholders with no active logic.

## 2. Code Quality Metrics

-   **Complexity**: The logic within individual methods is generally straightforward. However, the overall architectural complexity is high due to the tight coupling between the ViewModels and the data access layer (`SqlServerService`). Business logic is mixed between ViewModels (filtering) and the database (stored procedures), making it difficult to trace and maintain.

-   **Maintainability**: The "fat client" architecture significantly hinders maintainability.
    -   Any change to a stored procedure's signature or database schema requires a new client application release.
    -   The lack of a centralized API layer means that reusing the backend logic for another client (e.g., a web application) is not possible without a major rewrite.
    -   The initial codebase had duplicated `INotifyPropertyChanged` logic across most ViewModels. This has been addressed by refactoring them to inherit from a common `BaseViewModel`, which has improved code consistency and maintainability.

## 3. Security Analysis

The application has **critical security vulnerabilities** that must be addressed before any production deployment.

-   **Credential Management**:
    1.  **Hardcoded Connection String**: The database connection string is hardcoded in `App.xaml.cs`. This exposes the database server, username, and password in the source code.
    2.  **Hardcoded Credential Paths**: `GoogleCloudStorageUploader.cs` contains a hardcoded absolute path to a developer's local credentials file, which will fail on any other machine and exposes sensitive file paths.
    3.  **Bundled Credentials**: `GoogleCloudAuthHelper.cs` copies a Google Cloud service account key from the app package. This key can be easily extracted from the application package by reverse-engineering it, granting the attacker full access to the permissions associated with that service account.
-   **Authentication**:
    1.  **Plain-Text Passwords**: The `Usuario` and `Cliente` models contain plain-text `Contraseña` properties. The `VerificarCredencialesAsync` method sends this plain-text password to the database. Storing and handling passwords in this manner is a severe security risk.
-   **Best Practices Compliance**: The application does not comply with modern security best practices for credential management, authentication, or secrets handling.

## 4. Performance Bottlenecks

-   **Inefficient Data Loading**: The ViewModels frequently load entire tables from the database into memory before filtering them on the client side (e.g., `VMUbicacion`, `VMPedidos`). This approach is not performant and will lead to high memory consumption and slow UI responsiveness as the database grows.
-   **Lack of Pagination**: The application does not use pagination. When lists become large, loading all items at once will significantly degrade performance.
-   **Client-Side Filtering**: Relying on client-side filtering for large datasets is inefficient. Filtering should be performed on the database server via `WHERE` clauses in the stored procedures.

## 5. Scalability Assessment

The current two-tier architecture is **not scalable**.
-   **Direct Database Connections**: Each client connects directly to the database. This does not scale beyond a small number of users and can quickly exhaust the database's available connections.
-   **Centralization**: The lack of a central API layer makes it impossible to implement common scalability patterns like load balancing, caching, or connection pooling effectively.

## 6. Recommendations for Improvements

1.  **Architecture: Adopt a Three-Tier Architecture**
    -   **Action**: Create a separate ASP.NET Core Web API project to act as a backend layer.
    -   **Benefit**: This will decouple the client from the database, centralize business logic, improve security, and enable scalability. The client application would make HTTP requests to this new API instead of calling `SqlServerService` directly.

2.  **Security: Overhaul Credential and Secret Management**
    -   **Action**:
        -   Move all database and external service interactions to the new backend API.
        -   Store connection strings and API keys in the backend using a secure mechanism like Azure Key Vault, AWS Secrets Manager, or .NET's built-in Secret Manager for development.
        -   Implement a proper authentication system (e.g., using JWTs) in the API.
        -   **Immediately stop storing plain-text passwords.** Use a strong, one-way hashing algorithm like **Argon2** or **bcrypt** to store password hashes.
    -   **Benefit**: Prevents exposure of sensitive credentials on the client side and protects user passwords.

3.  **Performance: Implement Server-Side Operations**
    -   **Action**:
        -   Modify the stored procedures to accept parameters for filtering, sorting, and pagination (`OFFSET`/`FETCH`).
        -   Update the backend API to pass these parameters from the client to the database.
    -   **Benefit**: The database will only return the data the client needs, drastically reducing network traffic, client-side memory usage, and improving application speed.

4.  **Testing: Introduce a Test Suite**
    -   **Action**: Create a new xUnit or NUnit test project.
    -   **Benefit**:
        -   Write unit tests for ViewModel logic (mocking service calls).
        -   Write integration tests for the new API endpoints to ensure they behave correctly. This will improve code reliability and reduce regressions.

## 7. Dependency Audit

The project relies on the following key NuGet packages for its backend functionality:
-   `Microsoft.Data.SqlClient`
-   `Google.Cloud.Storage.V1`

**Recommendation**: Regularly audit these packages for updates, especially for security patches, using the Visual Studio NuGet Package Manager or a command-line tool like `dotnet list package --outdated`.