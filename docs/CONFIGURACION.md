# Administración inicial
Registra la cuenta mediante la API. Un operador autorizado de SQL Server debe comprobar la identidad y el id de esa cuenta antes de promoverla. En SSMS, sobre la base de desarrollo MoneyTrack, usa parámetros correspondientes a la cuenta comprobada y una transacción. El siguiente ejemplo requiere reemplazar el id y el correo. La condición exige que aún no exista ningún administrador.

    BEGIN TRANSACTION;
    DECLARE @id int = 0, @correo nvarchar(254) = N'';
    UPDATE Usuarios SET rol=N'Administrador'
    WHERE id_usuario=@id AND correo=@correo AND rol=N'Usuario'
      AND NOT EXISTS (SELECT 1 FROM Usuarios WITH (UPDLOCK,HOLDLOCK) WHERE rol=N'Administrador');
    IF @@ROWCOUNT=1 COMMIT ELSE ROLLBACK;

Inicia sesión de nuevo para recibir el rol actualizado. Para los cambios posteriores usa GET /api/admin/usuarios?pagina=1 y PUT /api/admin/usuarios/{id}/rol. El servidor comprueba el rol vigente en cada solicitud. La API impide cambiar el rol propio y protege al último administrador.
