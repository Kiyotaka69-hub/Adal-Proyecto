SET NOCOUNT ON;
SET XACT_ABORT OFF;
BEGIN TRANSACTION;
BEGIN TRY
    DECLARE @a int, @b int, @cat int;
    INSERT Usuarios(nombre, correo, password_hash, fecha_registro, rol)
    VALUES ('Demo A', CONCAT(NEWID(), '@example.invalid'), 'disabled', SYSUTCDATETIME(), 'Usuario');
    SET @a = SCOPE_IDENTITY();
    INSERT Usuarios(nombre, correo, password_hash, fecha_registro, rol)
    VALUES ('Demo B', CONCAT(NEWID(), '@example.invalid'), 'disabled', SYSUTCDATETIME(), 'Usuario');
    SET @b = SCOPE_IDENTITY();
    INSERT Categorias(id_usuario, nombre, tipo) VALUES (@a, 'Demo transporte', 'Gasto');
    SET @cat = SCOPE_IDENTITY();
    INSERT Movimientos(id_usuario,id_categoria,cantidad,tipo,fecha,es_recurrente)
    VALUES (@a,@cat,42.75,'Gasto','20261101',0);
    INSERT Presupuestos(id_usuario,id_categoria,limite,mes,anio,origen)
    VALUES (@a,@cat,100,11,2026,'Manual');
    BEGIN TRY
        INSERT Movimientos(id_usuario,id_categoria,cantidad,tipo,fecha,es_recurrente) VALUES (@b,@cat,10,'Gasto','20261101',0);
        THROW 51000, 'La restricción aceptó una fila inválida', 1;
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() NOT IN (547) THROW;
    END CATCH;
    BEGIN TRY
        INSERT Movimientos(id_usuario,id_categoria,cantidad,tipo,fecha,es_recurrente) VALUES (@a,@cat,-1,'Gasto','20261101',0);
        THROW 51000, 'La restricción aceptó una fila inválida', 1;
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() NOT IN (547) THROW;
    END CATCH;
    BEGIN TRY
        INSERT Presupuestos(id_usuario,id_categoria,limite,mes,anio,origen) VALUES (@a,@cat,100,11,2026,'Manual');
        THROW 51000, 'La restricción aceptó una fila inválida', 1;
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() NOT IN (2601,2627) THROW;
    END CATCH;
    ROLLBACK;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;
    THROW;
END CATCH;
