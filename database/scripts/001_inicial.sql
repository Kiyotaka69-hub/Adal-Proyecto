IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123041800_Inicial'
)
BEGIN
    CREATE TABLE [Usuarios] (
        [id_usuario] int NOT NULL IDENTITY,
        [nombre] nvarchar(120) NOT NULL,
        [correo] nvarchar(254) NOT NULL,
        [password_hash] nvarchar(100) NOT NULL,
        [fecha_registro] datetime2 NOT NULL,
        [rol] nvarchar(20) NOT NULL,
        CONSTRAINT [PK_Usuarios] PRIMARY KEY ([id_usuario]),
        CONSTRAINT [CK_Usuarios_1] CHECK (rol IN ('Usuario','Administrador'))
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123041800_Inicial'
)
BEGIN
    CREATE TABLE [Categorias] (
        [id_categoria] int NOT NULL IDENTITY,
        [id_usuario] int NOT NULL,
        [nombre] nvarchar(100) NOT NULL,
        [tipo] nvarchar(7) NOT NULL,
        CONSTRAINT [PK_Categorias] PRIMARY KEY ([id_categoria]),
        CONSTRAINT [AK_Categorias_id_usuario_id_categoria] UNIQUE ([id_usuario], [id_categoria]),
        CONSTRAINT [AK_Categorias_id_usuario_id_categoria_tipo] UNIQUE ([id_usuario], [id_categoria], [tipo]),
        CONSTRAINT [CK_Categorias_1] CHECK (tipo IN ('Ingreso','Gasto')),
        CONSTRAINT [FK_Categorias_Usuarios_id_usuario] FOREIGN KEY ([id_usuario]) REFERENCES [Usuarios] ([id_usuario]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123041800_Inicial'
)
BEGIN
    CREATE TABLE [ConversacionesIA] (
        [id_mensaje] int NOT NULL IDENTITY,
        [id_usuario] int NOT NULL,
        [rol] nvarchar(12) NOT NULL,
        [contenido] nvarchar(4000) NOT NULL,
        [fecha] datetime2 NOT NULL,
        CONSTRAINT [PK_ConversacionesIA] PRIMARY KEY ([id_mensaje]),
        CONSTRAINT [CK_ConversacionesIA_1] CHECK (rol IN ('Usuario','Asistente')),
        CONSTRAINT [FK_ConversacionesIA_Usuarios_id_usuario] FOREIGN KEY ([id_usuario]) REFERENCES [Usuarios] ([id_usuario]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123041800_Inicial'
)
BEGIN
    CREATE TABLE [Movimientos] (
        [id_movimiento] int NOT NULL IDENTITY,
        [id_usuario] int NOT NULL,
        [id_categoria] int NOT NULL,
        [cantidad] decimal(12,2) NOT NULL,
        [tipo] nvarchar(7) NOT NULL,
        [descripcion] nvarchar(500) NULL,
        [fecha] date NOT NULL,
        [es_recurrente] bit NOT NULL,
        CONSTRAINT [PK_Movimientos] PRIMARY KEY ([id_movimiento]),
        CONSTRAINT [CK_Movimientos_1] CHECK (cantidad > 0),
        CONSTRAINT [CK_Movimientos_2] CHECK (tipo IN ('Ingreso','Gasto')),
        CONSTRAINT [FK_Movimientos_Categorias_id_usuario_id_categoria_tipo] FOREIGN KEY ([id_usuario], [id_categoria], [tipo]) REFERENCES [Categorias] ([id_usuario], [id_categoria], [tipo]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Movimientos_Usuarios_id_usuario] FOREIGN KEY ([id_usuario]) REFERENCES [Usuarios] ([id_usuario]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123041800_Inicial'
)
BEGIN
    CREATE TABLE [Predicciones] (
        [id_prediccion] int NOT NULL IDENTITY,
        [id_usuario] int NOT NULL,
        [id_categoria] int NOT NULL,
        [mes] int NOT NULL,
        [anio] int NOT NULL,
        [monto_predicho] decimal(12,2) NOT NULL,
        [confianza] decimal(5,4) NOT NULL,
        [metodo] nvarchar(100) NOT NULL,
        [fecha_generada] datetime2 NOT NULL,
        CONSTRAINT [PK_Predicciones] PRIMARY KEY ([id_prediccion]),
        CONSTRAINT [CK_Predicciones_1] CHECK (mes BETWEEN 1 AND 12),
        CONSTRAINT [CK_Predicciones_2] CHECK (anio BETWEEN 1 AND 9999),
        CONSTRAINT [CK_Predicciones_3] CHECK (monto_predicho >= 0),
        CONSTRAINT [CK_Predicciones_4] CHECK (confianza BETWEEN 0 AND 1),
        CONSTRAINT [FK_Predicciones_Categorias_id_usuario_id_categoria] FOREIGN KEY ([id_usuario], [id_categoria]) REFERENCES [Categorias] ([id_usuario], [id_categoria]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Predicciones_Usuarios_id_usuario] FOREIGN KEY ([id_usuario]) REFERENCES [Usuarios] ([id_usuario]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123041800_Inicial'
)
BEGIN
    CREATE TABLE [Presupuestos] (
        [id_presupuesto] int NOT NULL IDENTITY,
        [id_usuario] int NOT NULL,
        [id_categoria] int NOT NULL,
        [limite] decimal(12,2) NOT NULL,
        [mes] int NOT NULL,
        [anio] int NOT NULL,
        [origen] nvarchar(6) NOT NULL,
        [monto_sugerido] decimal(12,2) NULL,
        CONSTRAINT [PK_Presupuestos] PRIMARY KEY ([id_presupuesto]),
        CONSTRAINT [CK_Presupuestos_1] CHECK (limite > 0),
        CONSTRAINT [CK_Presupuestos_2] CHECK (mes BETWEEN 1 AND 12),
        CONSTRAINT [CK_Presupuestos_3] CHECK (anio BETWEEN 1 AND 9999),
        CONSTRAINT [CK_Presupuestos_4] CHECK (origen IN ('Manual','IA')),
        CONSTRAINT [CK_Presupuestos_5] CHECK (monto_sugerido IS NULL OR monto_sugerido >= 0),
        CONSTRAINT [FK_Presupuestos_Categorias_id_usuario_id_categoria] FOREIGN KEY ([id_usuario], [id_categoria]) REFERENCES [Categorias] ([id_usuario], [id_categoria]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Presupuestos_Usuarios_id_usuario] FOREIGN KEY ([id_usuario]) REFERENCES [Usuarios] ([id_usuario]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123041800_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Categorias_id_usuario_id_categoria] ON [Categorias] ([id_usuario], [id_categoria]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123041800_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Categorias_id_usuario_id_categoria_tipo] ON [Categorias] ([id_usuario], [id_categoria], [tipo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123041800_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Categorias_id_usuario_nombre_tipo] ON [Categorias] ([id_usuario], [nombre], [tipo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123041800_Inicial'
)
BEGIN
    CREATE INDEX [IX_ConversacionesIA_id_usuario] ON [ConversacionesIA] ([id_usuario]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123041800_Inicial'
)
BEGIN
    CREATE INDEX [IX_Movimientos_id_usuario_fecha] ON [Movimientos] ([id_usuario], [fecha]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123041800_Inicial'
)
BEGIN
    CREATE INDEX [IX_Movimientos_id_usuario_id_categoria_tipo] ON [Movimientos] ([id_usuario], [id_categoria], [tipo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123041800_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Predicciones_id_usuario_id_categoria_mes_anio] ON [Predicciones] ([id_usuario], [id_categoria], [mes], [anio]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123041800_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Presupuestos_id_usuario_id_categoria_mes_anio] ON [Presupuestos] ([id_usuario], [id_categoria], [mes], [anio]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123041800_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Usuarios_correo] ON [Usuarios] ([correo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123041800_Inicial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261123041800_Inicial', N'8.0.34');
END;
GO

COMMIT;
GO

