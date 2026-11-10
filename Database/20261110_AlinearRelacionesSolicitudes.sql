/*
  Ejecutar solo después de respaldar la base de datos y verificar los nombres de tablas/columnas.
  SQL Server. Mantiene los datos históricos; WITH NOCHECK evita exigir que las filas antiguas
  cumplan la nueva relación, pero sí valida las nuevas inserciones/actualizaciones.
*/
IF OBJECT_ID(N'dbo.SolicitudesServicio', N'U') IS NULL
    THROW 51000, 'No existe dbo.SolicitudesServicio.', 1;
IF OBJECT_ID(N'dbo.Clientes', N'U') IS NULL
    THROW 51001, 'No existe dbo.Clientes.', 1;
IF OBJECT_ID(N'dbo.Conductores', N'U') IS NULL
    THROW 51002, 'No existe dbo.Conductores.', 1;
IF OBJECT_ID(N'dbo.Vehiculos', N'U') IS NULL
    THROW 51003, 'No existe dbo.Vehiculos.', 1;

-- El código antiguo usaba 0 como sustituto de NULL para conductor/vehículo.
-- Asegurar nulabilidad antes de reemplazar esos valores centinela.
ALTER TABLE dbo.SolicitudesServicio ALTER COLUMN ConductorId int NULL;
ALTER TABLE dbo.SolicitudesServicio ALTER COLUMN VehiculoId int NULL;
UPDATE dbo.SolicitudesServicio SET ConductorId = NULL WHERE ConductorId = 0;
UPDATE dbo.SolicitudesServicio SET VehiculoId = NULL WHERE VehiculoId = 0;

-- Quitar solo las claves foráneas que salen de estas columnas de SolicitudesServicio.
DECLARE @dropSql nvarchar(max) = N'';
SELECT @dropSql = @dropSql + N'ALTER TABLE dbo.SolicitudesServicio DROP CONSTRAINT ' + QUOTENAME(fk.name) + N';'
FROM sys.foreign_keys fk
JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
JOIN sys.columns col ON col.object_id = fkc.parent_object_id AND col.column_id = fkc.parent_column_id
WHERE fk.parent_object_id = OBJECT_ID(N'dbo.SolicitudesServicio')
  AND col.name IN (N'ClienteId', N'ConductorId', N'VehiculoId');
IF LEN(@dropSql) > 0 EXEC sp_executesql @dropSql;

-- Evita duplicar constraints si el script se vuelve a ejecutar.
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_SolicitudesServicio_Clientes')
    ALTER TABLE dbo.SolicitudesServicio WITH NOCHECK
      ADD CONSTRAINT FK_SolicitudesServicio_Clientes FOREIGN KEY (ClienteId) REFERENCES dbo.Clientes(Id);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_SolicitudesServicio_Conductores')
    ALTER TABLE dbo.SolicitudesServicio WITH NOCHECK
      ADD CONSTRAINT FK_SolicitudesServicio_Conductores FOREIGN KEY (ConductorId) REFERENCES dbo.Conductores(Id);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_SolicitudesServicio_Vehiculos')
    ALTER TABLE dbo.SolicitudesServicio WITH NOCHECK
      ADD CONSTRAINT FK_SolicitudesServicio_Vehiculos FOREIGN KEY (VehiculoId) REFERENCES dbo.Vehiculos(Id);

-- Tabla de última ubicación del conductor, usada por el mapa de pasajeros.
-- Se crea solo si no existe; revisar antes que la BD no tenga una tabla equivalente
-- con un nombre distinto o un esquema personalizado.
IF OBJECT_ID(N'dbo.UbicacionesConductor', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UbicacionesConductor
    (
        ConductorId int NOT NULL,
        Latitud float NOT NULL,
        Longitud float NOT NULL,
        FechaActualizacion datetime2 NOT NULL,
        CONSTRAINT PK_UbicacionesConductor PRIMARY KEY (ConductorId),
        CONSTRAINT FK_UbicacionesConductor_Conductores
            FOREIGN KEY (ConductorId) REFERENCES dbo.Conductores(Id)
    );
END;
