# 🧾 Sistema Contable — P1SC08

> Sistema de escritorio para gestión de caja, ventas e inventario dirigido a pequeñas y medianas empresas.

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-blue?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-Windows%20Forms-purple?logo=csharp)
![SQL Server](https://img.shields.io/badge/SQL%20Server-Express-red?logo=microsoftsqlserver)
![Tests](https://img.shields.io/badge/Tests-23%20unitarios-green?logo=visualstudio)
![Metodología](https://img.shields.io/badge/Metodolog%C3%ADa-Scrum-orange)

---

## 📋 Descripción

Sistema contable local desarrollado en **C# con Windows Forms** y **SQL Server Express**. Cubre el módulo de productos (CRUD completo), autenticación segura con hash SHA-256, y la base del módulo de ventas.

---

## 🗂️ Estructura del Repositorio

```
P1SC08/
├── P1SC08/                         # Proyecto principal
│   ├── Classes/
│   │   ├── clsBusco.cs             # Conexión BD, secuencias, hashing SHA-256
│   │   └── ConvertImage.cs         # Utilidad imagen ↔ byte array
│   ├── Formularios/
│   │   ├── frmStarts.cs            # Login con autenticación SHA-256
│   │   ├── frmMenu.cs              # Menú principal
│   │   ├── frmProductos.cs         # CRUD de productos
│   │   └── frmVENPROD.cs           # Buscador/visor de productos
│   ├── SQL/
│   │   └── DBpractica04_CreateScript.sql   # Script completo de BD
│   ├── Resources/                  # Íconos e imágenes del sistema
│   ├── Program.cs
│   └── App.config
├── P1SC08.Tests/                   # Suite de pruebas unitarias (MSTest)
│   └── P1SC08Tests.cs              # 23 casos de prueba
├── docs/
│   └── SistemaContable_Documentacion_Tecnica.docx
├── evidencias/                     # Capturas de ejecución de pruebas
│   └── .gitkeep
├── .gitignore
└── README.md
```

---

## 🛠️ Tecnologías

| Capa           | Tecnología                    |
|----------------|-------------------------------|
| Presentación   | Windows Forms (.NET 4.8)      |
| Lógica         | C#                            |
| Base de datos  | SQL Server Express (local)    |
| Pruebas        | MSTest + xUnit                |
| CI/CD          | GitHub Actions                |
| Gestión        | Jira / Trello                 |

---

## ⚙️ Requisitos Previos

- Windows 10 / 11
- Visual Studio 2022 (con carga de trabajo .NET Desktop)
- SQL Server Express (instancia `localhost`)
- .NET Framework 4.8

---

## 🚀 Instalación y Configuración

### 1. Clonar el repositorio

```bash
git clone https://github.com/[tu-usuario]/P1SC08.git
cd P1SC08
```

### 2. Crear la base de datos

Abre **SQL Server Management Studio (SSMS)** y ejecuta:

```sql
-- Ejecutar el script completo:
P1SC08/SQL/DBpractica04_CreateScript.sql
```

Esto crea la base de datos `DBpractica04` con todas las tablas, datos de prueba, vistas y el stored procedure `usp_InsertarVenta`.

### 3. Verificar la cadena de conexión

En `Classes/clsBusco.cs`, asegúrate de que la conexión apunte a tu instancia:

```csharp
public static string db = @"Data Source=localhost;
                             Initial Catalog=DBpractica04;
                             Integrated Security=true;";
```

> Si usas una instancia nombrada (ej. `SQLEXPRESS`), cámbiala a `Data Source=localhost\SQLEXPRESS`.

### 4. Compilar y ejecutar

Abre `P1SC08.sln` en Visual Studio 2022 → `F5` o `Ctrl+F5`.

### 5. Credenciales de prueba

| Usuario  | Contraseña  | Rol           |
|----------|-------------|---------------|
| `admin`  | `admin123`  | Administrador |
| `cajero` | `cajero123` | Cajero        |

---

## 🧪 Ejecutar Pruebas

### Desde Visual Studio

1. Abrir **Test Explorer** (`Ctrl+E, T`)
2. Click en **Run All Tests**

### Desde consola

```bash
dotnet test P1SC08.Tests/P1SC08.Tests.csproj
```

### Suites disponibles

| Suite                    | Casos | Cubre                                  |
|--------------------------|-------|----------------------------------------|
| `SeguridadTests`         | 7     | Hash SHA-256, verificación contraseñas |
| `ConvertImageTests`      | 3     | Conversión imagen ↔ bytes              |
| `CalculosVentaTests`     | 7     | Subtotales, ITBIS, descuentos          |
| `ValidacionCamposTests`  | 6     | Stock, precios, campos obligatorios    |
| **Total**                | **23**|                                        |

---

## 🐛 Bugs Corregidos

| ID      | Archivo            | Descripción                                              | Severidad |
|---------|--------------------|----------------------------------------------------------|-----------|
| BUG-001 | `frmProductos.cs`  | Alias incorrectos en SELECT (`PRECIO`/`BARRA`)           | Crítico   |
| BUG-002 | `frmProductos.cs`  | `DialogResult` (clase) en vez de variable → nunca borraba| Crítico   |
| BUG-003 | `clsBusco.cs`      | `Dispose()`/`Close()` tras `return` → conexión nunca cierra | Alto   |
| BUG-004 | `frmStarts.cs`     | Contraseña en texto plano → reemplazado por SHA-256      | Seguridad |
| BUG-005 | `frmProductos.cs`  | Validaciones sin `MessageBox` → usuario sin retroalimentación | Medio |

---

## 📌 Estado del Proyecto — Release 1.0

| Módulo               | Estado         |
|----------------------|----------------|
| Autenticación Login  | ✅ Completo    |
| CRUD Productos       | ✅ Completo    |
| Búsqueda Inventario  | ✅ Completo    |
| Módulo Ventas        | 🔄 En progreso |
| Generación Factura   | 📋 Pendiente   |
| Reportes             | 📋 Pendiente   |

---

## 📁 Documentación

- 📄 [Documentación Técnica Completa](docs/SistemaContable_Documentacion_Tecnica.docx)
- 🗂️ [Historias de Usuario en Jira](#) *(link al board)*
- 🧪 [Evidencias de Pruebas](evidencias/)

---

## 👤 Autor

**[Tu Nombre]** — Desarrollador, QA e Implementador  
Proyecto Final — Programación / Sistemas  
ITLA — 2025
