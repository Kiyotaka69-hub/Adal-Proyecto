# Historial de cambios

## En desarrollo

- Incorporados el plan por fases y la matriz de aceptación basada en el documento de presentación.


- Iniciada la estructura del repositorio con README, exclusiones de secretos y plantilla de entorno.

- Solución .NET 8 con cinco proyectos y frontend Expo Router con Axios, SecureStore y correcciones de TypeScript y ESLint.

- Incorporado esquema SQL Server versionado por EF Core con claves de propietario, controles de importe y periodo e índices.

- Validada la migración inicial sobre SQL Server Express LocalDB con pruebas transaccionales de integridad.

- API de sesión, categorías y movimientos verificada con dos cuentas en SQL Server.

Fase 3: agregados CRUD de presupuestos, GET /api/dashboard/resumen y GET /api/dashboard/estadisticas. Saldo histórico, ingresos y gastos mensuales, agrupación por categoría y progreso de cada presupuesto calculados en SQL Server. Prueba HTTPS ampliada y ejecutada: duplicado 409, aislamiento, gasto de 30 MXN, saldo -30 y avance de 30. Sin cambios de esquema. Siguiente tarea: endpoints del asistente.
