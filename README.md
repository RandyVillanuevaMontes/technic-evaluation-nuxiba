# Proyecto API de Logins y Reporte CSV

---

## 1. Requisitos Previos

Antes de comenzar:

- **Docker** – Para levantar el contenedor de SQL Server.  
- **.NET 7 SDK o superior** – Para ejecutar la API.  
- **SQL Server Management Studio (SSMS) o Azure Data Studio** – Para conectarte a la base de datos.  
- **Postman o navegador** – Para probar los endpoints de la API.  

---

## 2. Levantar Contenedor de SQL Server

Ejecuta:

```bash
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=YourStrong!Passw0rd' \
-p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2019-latest
```

Verifica que esté corriendo:
```
docker ps
```

| Campo      | Valor                 |
| ---------- | --------------------- |
| Servidor   | `localhost,1433`      |
| Usuario    | `sa`                  |
| Contraseña | `YourStrong!Passw0rd` |

```
-- Crear base de datos
CREATE DATABASE CCenterRIA_DB;
GO

USE CCenterRIA_DB;
GO

-- Tabla de Áreas
CREATE TABLE ccRIACat_Areas (
    IDArea INT PRIMARY KEY,
    AreaName NVARCHAR(100) NULL,
    StatusArea INT NOT NULL,
    fecha DATETIME NULL
);

-- Tabla de Usuarios
CREATE TABLE ccUsers (
    User_id INT PRIMARY KEY,
    Login NVARCHAR(50) NULL,
    Nombres NVARCHAR(100) NULL,
    ApellidoPaterno NVARCHAR(100) NULL,
    ApellidoMaterno NVARCHAR(100) NULL,
    Area_id INT NOT NULL,
    FOREIGN KEY (Area_id) REFERENCES ccRIACat_Areas(IDArea)
);

-- Tabla de Logins
CREATE TABLE ccloglogin (
    LoginId INT PRIMARY KEY IDENTITY,
    User_id INT NOT NULL,
    Extension INT NOT NULL,
    TipoMov INT NOT NULL,
    fecha DATETIME NULL,
    FOREIGN KEY (User_id) REFERENCES ccUsers(User_id)
);

```
## Insertar Datos de Prueba
```
-- Áreas
INSERT INTO ccRIACat_Areas VALUES (1, 'TI', 1, GETDATE());
INSERT INTO ccRIACat_Areas VALUES (2, 'RRHH', 1, GETDATE());

-- Usuarios
INSERT INTO ccUsers VALUES (1, 'usuario1', 'Randy', 'Villanueva', 'N/A', 1);
INSERT INTO ccUsers VALUES (2, 'usuario2', 'Ana', 'Gomez', 'N/A', 2);

-- Logins
INSERT INTO ccloglogin VALUES (1, 1, 101, 1, DATEADD(HOUR, -8, GETDATE())); -- Login
INSERT INTO ccloglogin VALUES (1, 1, 101, 0, DATEADD(HOUR, -2, GETDATE())); -- Logout
INSERT INTO ccloglogin VALUES (2, 2, 102, 1, DATEADD(HOUR, -9, GETDATE()));
INSERT INTO ccloglogin VALUES (2, 2, 102, 0, DATEADD(HOUR, -1, GETDATE()));
```
## Configurar y Ejecutar la API
```
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=CCenterRIA_DB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"
}
```
Ejecuta la API:

```
dotnet run --project Evaluation_Nuxiba
```

## Endpoints Disponibles

| Método | Endpoint              | Descripción                       |
| ------ | --------------------- | --------------------------------- |
| GET    | `/logins`             | Obtener todos los logins.         |
| POST   | `/logins`             | Registrar un login/logout.        |
| PUT    | `/logins/{id}`        | Actualizar un login.              |
| DELETE | `/logins/{id}`        | Eliminar un login.                |
| GET    | `/logins/reporte-csv` | Generar CSV con horas trabajadas. |
