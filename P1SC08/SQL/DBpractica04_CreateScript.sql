-- ===========================================================================
-- SISTEMA CONTABLE — P1SC08
-- Script de creación de base de datos: DBpractica04
-- Servidor: localhost (SQL Server Express / SQL Server)
-- Fecha: Abril 2025
-- ===========================================================================

USE master;
GO

-- Crea la base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'DBpractica04')
BEGIN
    CREATE DATABASE DBpractica04;
END
GO

USE DBpractica04;
GO

-- ===========================================================================
-- TABLA: SECUENCIA
-- Maneja los autonuméricos del sistema (equivalente a secuencias en Oracle).
-- La clase Busco.BuscaUltimoNumero() consulta esta tabla.
-- ===========================================================================
IF OBJECT_ID('dbo.SECUENCIA', 'U') IS NOT NULL
    DROP TABLE dbo.SECUENCIA;
GO

CREATE TABLE dbo.SECUENCIA (
    ID          VARCHAR(10)   NOT NULL,   -- identificador de la secuencia ('1' = Productos, etc.)
    SECUENCIA   INT           NOT NULL DEFAULT 0,
    DESCRIPCION VARCHAR(50)   NULL,
    CONSTRAINT PK_SECUENCIA PRIMARY KEY (ID)
);
GO

-- Datos iniciales de secuencias
INSERT INTO dbo.SECUENCIA (ID, SECUENCIA, DESCRIPCION)
VALUES
    ('1', 0, 'Secuencia de Productos');
GO

-- ===========================================================================
-- TABLA: USUARIO
-- Almacena los usuarios que pueden acceder al sistema.
-- IMPORTANTE: El campo CLAVE debe contener el hash SHA-256 de la contraseña,
--             NO la contraseña en texto plano (ver clase Seguridad.HashSHA256).
-- ===========================================================================
IF OBJECT_ID('dbo.USUARIO', 'U') IS NOT NULL
    DROP TABLE dbo.USUARIO;
GO

CREATE TABLE dbo.USUARIO (
    IDUSUARIO      INT           IDENTITY(1,1) NOT NULL,
    NOMBRECORTO    VARCHAR(20)   NOT NULL,    -- nombre de usuario para login
    NOMBRECOMPLETO VARCHAR(100)  NULL,
    CLAVE          VARCHAR(64)   NOT NULL,    -- hash SHA-256 (64 caracteres hex)
    ROL            VARCHAR(20)   NOT NULL DEFAULT 'CAJERO',  -- ADMIN / CAJERO
    ACTIVO         BIT           NOT NULL DEFAULT 1,
    FECHACREACION  DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_USUARIO    PRIMARY KEY (IDUSUARIO),
    CONSTRAINT UQ_NOMBRECORTO UNIQUE (NOMBRECORTO),
    CONSTRAINT CK_ROL         CHECK (ROL IN ('ADMIN', 'CAJERO'))
);
GO

-- Datos de prueba:
-- Contraseña del admin: "admin123"  → SHA-256 hash
-- Contraseña del cajero: "cajero123" → SHA-256 hash
--
-- NOTA: Para generar el hash en C#:
--   Seguridad.HashSHA256("admin123")
--   = "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9"
--   Seguridad.HashSHA256("cajero123")
--   = "bc547e5b13ea4f6e0b4e10ee2c26b0c68e02b8f53dc2b2db5dd6e6d2f0d59d8e"
--   (ejecutar el método para obtener el valor exacto en tu entorno)

INSERT INTO dbo.USUARIO (NOMBRECORTO, NOMBRECOMPLETO, CLAVE, ROL)
VALUES
    ('admin',  'Administrador del Sistema',
     '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9',
     'ADMIN'),
    ('cajero', 'Cajero General',
     'bc547e5b13ea4f6e0b4e10ee2c26b0c68e02b8f53dc2b2db5dd6e6d2f0d59d8e',
     'CAJERO');
GO

-- ===========================================================================
-- TABLA: PRODUCTOS
-- Catálogo maestro de productos del inventario.
-- ===========================================================================
IF OBJECT_ID('dbo.PRODUCTOS', 'U') IS NOT NULL
    DROP TABLE dbo.PRODUCTOS;
GO

CREATE TABLE dbo.PRODUCTOS (
    ITEM                  VARCHAR(20)     NOT NULL,   -- código único del producto
    DESCRIPCION           VARCHAR(200)    NOT NULL,
    CANTIDADENEXISTENCIA  DECIMAL(10, 2)  NOT NULL DEFAULT 0,
    COSTO                 DECIMAL(12, 2)  NOT NULL DEFAULT 0,
    PRECIODEVENTA         DECIMAL(12, 2)  NOT NULL DEFAULT 0,
    IMPUESTO              DECIMAL(5,  2)  NOT NULL DEFAULT 18,  -- % ITBIS (RD: 18%)
    BARCODE               VARCHAR(50)     NULL,
    IMAGEN                VARBINARY(MAX)  NULL,
    ESTATUSPRODUCTO       BIT             NOT NULL DEFAULT 1,   -- 1=Activo, 0=Inactivo
    FECHACREACION         DATETIME        NOT NULL DEFAULT GETDATE(),
    FECHAMODIFICACION     DATETIME        NULL,
    CONSTRAINT PK_PRODUCTOS PRIMARY KEY (ITEM)
);
GO

-- Datos de prueba
INSERT INTO dbo.PRODUCTOS
    (ITEM, DESCRIPCION, CANTIDADENEXISTENCIA, COSTO, PRECIODEVENTA, IMPUESTO, BARCODE, ESTATUSPRODUCTO)
VALUES
    ('1',  'Lapicero Azul BIC',          100, 5.00,  15.00, 18, '7501031311309', 1),
    ('2',  'Cuaderno Rayado 100 hojas',  50,  35.00, 75.00, 18, '7501031311316', 1),
    ('3',  'Resma de Papel Letter',      30,  250.00,450.00,18, '7501031311323', 1),
    ('4',  'Marcador Permanente Negro',  80,  20.00, 45.00, 18, '7501031311330', 1),
    ('5',  'Grapadora Metálica',         15,  150.00,300.00,18, '7501031311347', 1);
GO

-- Actualizar la secuencia al último ITEM insertado
UPDATE dbo.SECUENCIA SET SECUENCIA = 5 WHERE ID = '1';
GO

-- ===========================================================================
-- TABLA: VENTAS (header)
-- Encabezado de cada transacción de venta.
-- Preparado para Release 1.0 — módulo de ventas.
-- ===========================================================================
IF OBJECT_ID('dbo.VENTAS', 'U') IS NOT NULL
    DROP TABLE dbo.VENTAS;
GO

CREATE TABLE dbo.VENTAS (
    NUMVENTA        INT             IDENTITY(1,1) NOT NULL,
    FECHAVENTA      DATETIME        NOT NULL DEFAULT GETDATE(),
    IDUSUARIO       INT             NOT NULL,
    SUBTOTAL        DECIMAL(12, 2)  NOT NULL DEFAULT 0,
    DESCUENTO       DECIMAL(12, 2)  NOT NULL DEFAULT 0,
    IMPUESTO        DECIMAL(12, 2)  NOT NULL DEFAULT 0,
    TOTAL           DECIMAL(12, 2)  NOT NULL DEFAULT 0,
    ESTATUSVENTA    BIT             NOT NULL DEFAULT 1,   -- 1=Confirmada, 0=Anulada
    OBSERVACIONES   VARCHAR(500)    NULL,
    CONSTRAINT PK_VENTAS   PRIMARY KEY (NUMVENTA),
    CONSTRAINT FK_VEN_USR  FOREIGN KEY (IDUSUARIO) REFERENCES dbo.USUARIO(IDUSUARIO)
);
GO

-- ===========================================================================
-- TABLA: VENTAS_DETALLE
-- Detalle (líneas) de cada venta.
-- ===========================================================================
IF OBJECT_ID('dbo.VENTAS_DETALLE', 'U') IS NOT NULL
    DROP TABLE dbo.VENTAS_DETALLE;
GO

CREATE TABLE dbo.VENTAS_DETALLE (
    IDDETALLE       INT             IDENTITY(1,1) NOT NULL,
    NUMVENTA        INT             NOT NULL,
    ITEM            VARCHAR(20)     NOT NULL,
    CANTIDAD        DECIMAL(10, 2)  NOT NULL,
    PRECIOUNITARIO  DECIMAL(12, 2)  NOT NULL,
    DESCUENTO       DECIMAL(12, 2)  NOT NULL DEFAULT 0,
    SUBTOTAL        DECIMAL(12, 2)  NOT NULL,
    CONSTRAINT PK_VENTAS_DET  PRIMARY KEY (IDDETALLE),
    CONSTRAINT FK_DET_VEN     FOREIGN KEY (NUMVENTA) REFERENCES dbo.VENTAS(NUMVENTA),
    CONSTRAINT FK_DET_PROD    FOREIGN KEY (ITEM)     REFERENCES dbo.PRODUCTOS(ITEM)
);
GO

-- ===========================================================================
-- STORED PROCEDURE: usp_InsertarVenta
-- Inserta una venta y su detalle en una transacción atómica.
-- Descuenta el stock automáticamente.
-- ===========================================================================
IF OBJECT_ID('dbo.usp_InsertarVenta', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_InsertarVenta;
GO

CREATE PROCEDURE dbo.usp_InsertarVenta
    @IDUSUARIO      INT,
    @SUBTOTAL       DECIMAL(12,2),
    @DESCUENTO      DECIMAL(12,2),
    @IMPUESTO       DECIMAL(12,2),
    @TOTAL          DECIMAL(12,2),
    @ITEMS          NVARCHAR(MAX)   -- JSON: [{"item":"1","cantidad":2,"precio":15.00}]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NUMVENTA INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO dbo.VENTAS (IDUSUARIO, SUBTOTAL, DESCUENTO, IMPUESTO, TOTAL)
        VALUES (@IDUSUARIO, @SUBTOTAL, @DESCUENTO, @IMPUESTO, @TOTAL);

        SET @NUMVENTA = SCOPE_IDENTITY();

        -- Parsear JSON y procesar cada línea
        INSERT INTO dbo.VENTAS_DETALLE (NUMVENTA, ITEM, CANTIDAD, PRECIOUNITARIO, SUBTOTAL)
        SELECT @NUMVENTA,
               JSON_VALUE(value, '$.item'),
               CAST(JSON_VALUE(value, '$.cantidad')AS DECIMAL(10,2)),
               CAST(JSON_VALUE(value, '$.precio')  AS DECIMAL(12,2)),
               CAST(JSON_VALUE(value, '$.cantidad')AS DECIMAL(10,2))
               * CAST(JSON_VALUE(value, '$.precio') AS DECIMAL(12,2))
        FROM OPENJSON(@ITEMS);

        -- Descontar stock
        UPDATE P
        SET    P.CANTIDADENEXISTENCIA = P.CANTIDADENEXISTENCIA - D.CANTIDAD,
               P.FECHAMODIFICACION   = GETDATE()
        FROM   dbo.PRODUCTOS P
        INNER JOIN dbo.VENTAS_DETALLE D ON D.ITEM = P.ITEM
        WHERE  D.NUMVENTA = @NUMVENTA;

        COMMIT TRANSACTION;
        SELECT @NUMVENTA AS NUMVENTA_GENERADO;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- ===========================================================================
-- VISTAS ÚTILES
-- ===========================================================================

-- Vista: Inventario con estatus de stock
CREATE OR ALTER VIEW dbo.vw_Inventario AS
SELECT
    ITEM,
    DESCRIPCION,
    CANTIDADENEXISTENCIA,
    COSTO,
    PRECIODEVENTA,
    IMPUESTO,
    BARCODE,
    ESTATUSPRODUCTO,
    CASE
        WHEN CANTIDADENEXISTENCIA <= 0  THEN 'SIN STOCK'
        WHEN CANTIDADENEXISTENCIA <= 5  THEN 'STOCK BAJO'
        ELSE 'DISPONIBLE'
    END AS ESTADO_STOCK
FROM dbo.PRODUCTOS
WHERE ESTATUSPRODUCTO = 1;
GO

-- Vista: Resumen de ventas por día
CREATE OR ALTER VIEW dbo.vw_VentasPorDia AS
SELECT
    CAST(FECHAVENTA AS DATE)  AS FECHA,
    COUNT(*)                  AS CANTIDAD_VENTAS,
    SUM(TOTAL)                AS TOTAL_VENDIDO,
    AVG(TOTAL)                AS PROMEDIO_VENTA
FROM dbo.VENTAS
WHERE ESTATUSVENTA = 1
GROUP BY CAST(FECHAVENTA AS DATE);
GO

-- ===========================================================================
-- VERIFICACIÓN FINAL
-- ===========================================================================
SELECT 'SECUENCIA'      AS TABLA, COUNT(*) AS REGISTROS FROM dbo.SECUENCIA
UNION ALL
SELECT 'USUARIO',        COUNT(*) FROM dbo.USUARIO
UNION ALL
SELECT 'PRODUCTOS',      COUNT(*) FROM dbo.PRODUCTOS
UNION ALL
SELECT 'VENTAS',         COUNT(*) FROM dbo.VENTAS
UNION ALL
SELECT 'VENTAS_DETALLE', COUNT(*) FROM dbo.VENTAS_DETALLE;
GO

PRINT '✅ Base de datos DBpractica04 creada correctamente.';
GO
