# Backend Documentation for AppTransporte

This document provides a comprehensive overview of the backend architecture, components, and logic for the AppTransporte application.

## 1. Architecture Overview

The AppTransporte application utilizes a **two-tier (fat client)** architecture. The backend logic is not deployed as a separate web service or API. Instead, it is embedded directly within the .NET MAUI client application.

-   **Data Access Layer**: The core of the backend is the `AppTransporte.model.SqlServerService` class. This class acts as a data access layer, encapsulating all communication with the Microsoft SQL Server database. It exclusively uses stored procedures to perform CRUD (Create, Read, Update, Delete) operations, which centralizes the database interaction logic.
-   **Cloud Integration**: The application integrates with Google Cloud Storage for file and evidence uploads, managed by the `GoogleCloudStorageUploader` class.
-   **Pattern**: Within the .NET MAUI application, the Model-View-ViewModel (MVVM) pattern is used. The backend logic (database services, cloud integration) resides in the **Model** layer and is consumed directly by the **ViewModels**. This makes the ViewModels responsible for orchestrating calls to the backend services and managing the application's state.

## 2. Technology Stack

-   **Programming Language**: C#
-   **Framework**: .NET MAUI (hosting the embedded backend logic)
-   **Database**: Microsoft SQL Server
-   **External Services**: Google Cloud Storage
-   **Key Libraries**:
    -   `Microsoft.Data.SqlClient`: For all direct database communication.
    -   `Google.Cloud.Storage.V1`: For interacting with the Google Cloud Storage API.

## 3. API Documentation (Service Layer)

Since there is no REST/HTTP API, the public methods of the service classes constitute the application's backend API.

### `SqlServerService` Methods

This class provides the primary interface to the database.

| Method                          | Description                                                                 |
| ------------------------------- | --------------------------------------------------------------------------- |
| `AgregarClienteAsync`           | Adds a new client to the database.                                          |
| `ModificarClienteAsync`         | Modifies an existing client's details.                                      |
| `eliminarClienteAsync`          | Deletes a client from the database.                                         |
| `ObtenerClientesAsync`          | Retrieves a list of all clients.                                            |
| `ObtenerClientePorUsuarioAsync` | Retrieves client details for a specific user ID.                            |
| `AgregarTrabajadorAsync`        | Adds a new worker (driver, assistant) to the database.                      |
| `ModificarTrabajadorAsync`      | Modifies an existing worker's details.                                      |
| `eliminarTrabajadorAsync`       | Deletes a worker from the database.                                         |
| `ObtenerTrabajadoresAsync`      | Retrieves a list of workers, optionally filtered by category.               |
| `AgregarSolicitudAsync`         | Creates a new service request.                                              |
| `ObtenerSolicitudesAsync`       | Retrieves service requests, optionally filtered by client ID.               |
| `CrearPedidoAsync`              | Creates a new order from a service request.                                 |
| `ListarPedidosAdminAsync`       | Retrieves a list of all orders for an admin view.                           |
| `ListarPedidosPorUsuario`       | Retrieves all orders for a specific user.                                   |
| `ObtenerViajesAsync`            | Retrieves a detailed list of all trips.                                     |
| `ObtenerViajesModAsync`         | Retrieves trips filtered by order ID or user ID.                            |
| `ObtenerEstadosViaje`           | Retrieves the full tracking history for all trips.                          |
| `AgregarVehiculo`               | Adds a new vehicle (Tracto or Cisterna).                                    |
| `ObtenerTractoAsync`            | Retrieves a list of tractor vehicles.                                       |
| `ObtenerCisternaAsync`          | Retrieves a list of tanker vehicles.                                        |
| `AgregarUbicacionAsync`         | Adds a new origin/destination location.                                     |
| `ModificarUbicacionAsync`       | Modifies an existing location.                                              |
| `eliminarUbicacionAsync`        | Deletes a location.                                                         |
| `ObtenerUbicacionesAsync`       | Retrieves a list of all locations.                                          |
| `VerificarCredencialesAsync`    | Authenticates a user against their username and password.                   |
| `obtenerTipoUser`               | Retrieves the user type (role) description from an ID.                      |

### `GoogleCloudStorageUploader` Methods

| Method            | Description                                      |
| ----------------- | ------------------------------------------------ |
| `UploadFileAsync` | Uploads a file stream to Google Cloud Storage.   |

## 4. Database Schema

The database schema is inferred from the data models in `AppTransporte/model/` and the stored procedure calls in `SqlServerService`. The design relies heavily on stored procedures for data manipulation.

**Main Tables (Inferred):**

-   `Cliente`: Stores client information.
-   `Trabajador`: Stores employee information (drivers, assistants).
-   `Usuario`: Stores user account credentials and links to a `Persona` and `TipoUsuario`.
-   `Persona`: A base table for personal information, shared by `Cliente` and `Trabajador`.
-   `Solicitud`: Stores initial service requests from clients.
-   `Pedido`: Stores approved orders, generated from `Solicitud`.
-   `Viaje`: Stores individual trips that make up an order.
-   `Vehiculo` (split into `Tracto` and `Cisterna` tables): Stores vehicle information.
-   `Ubicacion` (as `Origen` and `Destino`): Stores location data.
-   `Seguimiento`: Logs the status history for each `Viaje`.
-   `Servicio`: Defines additional services that can be part of an order.
-   **Lookup Tables**: `Categoria`, `TipoDocumento`, `TipoUsuario`, `EstadoPedido`, `EstadoSolicitud`, `EstadoViaje`.
-   **Join Tables**: `ServicioPedido`, `TrabajadorViaje`.

## 5. Environment Configuration

-   **Database Connection String**: The `SqlServerService` class requires a valid SQL Server connection string to be passed to its constructor. This is configured during application startup in `App.xaml.cs`.
-   **Google Cloud Credentials**:
    -   The application requires a Google Cloud service account key file named `projecto-rocketbot-71a45741c162.json`.
    -   **SECURITY WARNING**: The current implementation hardcodes the path to this file (`GoogleCloudStorageUploader`) or bundles it directly in the app package (`GoogleCloudAuthHelper`). This is a major security risk. In a production environment, credentials must be managed securely using a proper secrets management service or by authenticating through a secure backend API.

## 6. Setup Instructions

1.  **Database Setup**:
    -   Set up a Microsoft SQL Server instance.
    -   Create the database schema and tables based on the models defined in the project.
    -   Deploy all stored procedures referenced in `SqlServerService.cs` to the database.
2.  **Application Configuration**:
    -   In `App.xaml.cs`, update the `SqlServerService` instantiation with the correct database connection string.
    -   To enable file uploads, place the Google Cloud service account key file (`projecto-rocketbot-71a45741c162.json`) in the root of the .NET MAUI project and set its build action to `MauiAsset`.
3.  **Build and Run**:
    -   Restore NuGet packages.
    -   Build and run the project on the desired platform (Android, iOS, Windows).

## 7. Authentication & Authorization

-   **Authentication**: User login is handled by the `VerificarCredencialesAsync` method, which calls the `pa_verificarCredenciales` stored procedure. It validates the provided username and password.
-   **Authorization**: Upon successful login, the method returns a `UsuarioResponse` object containing the user's ID and a `idTipoUsuario` (user type ID). This user type ID is used throughout the application to control access to different views and functionalities (e.g., loading all orders for an admin vs. only specific orders for a client).

## 8. Business Logic

-   **Service Request Workflow**: Clients create `Solicitudes` (requests). An administrator reviews them and can convert an approved `Solicitud` into a `Pedido` (order).
-   **Order Fulfillment**: Each `Pedido` is fulfilled via one or more `Viajes` (trips). Vehicles and workers are assigned to each trip.
-   **Trip Tracking**: The progress of each `Viaje` is monitored through `Seguimiento` events, which log the status (e.g., "En route"), timestamp, and optional evidence (uploaded to Google Cloud Storage).

## 9. External Dependencies

-   **Microsoft SQL Server**: The primary data store for all application data.
-   **Google Cloud Storage**: Used for storing binary file uploads, such as evidence for trip tracking.

## 10. Error Handling

-   Error handling is implemented using `try-catch` blocks within the service and ViewModel methods.
-   Exceptions are currently written to the debug console via `Console.WriteLine`. For a production application, a structured logging framework (like Serilog or NLog) should be implemented to capture errors in a more robust and manageable way.

## 11. Testing

-   The repository does not contain any unit or integration tests for the backend logic.
-   It is highly recommended to create a separate test project and add tests for:
    -   **`SqlServerService`**: To verify the correctness of database interactions (this would require a test database or mocking the `SqlConnection`).
    -   **ViewModels**: To ensure the filtering, sorting, and state management logic is correct.
    -   Frameworks like **xUnit** or **NUnit** with a mocking library like **Moq** would be suitable.

## 12. Known Issues & Limitations

-   **Security**: The management of database connection strings and Google Cloud credentials is not secure for a production environment. Plain-text passwords are saved and used, which is a critical vulnerability.
-   **Scalability**: The "fat client" architecture with direct database connections from each client does not scale well and can lead to performance bottlenecks and security risks. A dedicated backend API would be a more scalable and secure solution.
-   **Maintainability**: The backend logic is tightly coupled to the client application, making it difficult to reuse for other platforms (e.g., a web portal) or to update independently.
-   **Performance**: The application often loads entire tables of data from the database and performs filtering on the client side. For large datasets, this is inefficient. Implementing server-side filtering and pagination in the stored procedures is recommended.