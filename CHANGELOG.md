# Historial de cambios

## En desarrollo

- Incorporados el plan por fases y la matriz de aceptación basada en el documento de presentación.


- Iniciada la estructura del repositorio con README, exclusiones de secretos y plantilla de entorno.

- Solución .NET 8 con cinco proyectos y frontend Expo Router con Axios, SecureStore y correcciones de TypeScript y ESLint.

- Incorporado esquema SQL Server versionado por EF Core con claves de propietario, controles de importe y periodo e índices.

- Validada la migración inicial sobre SQL Server Express LocalDB con pruebas transaccionales de integridad.

- API de sesión, categorías y movimientos verificada con dos cuentas en SQL Server.

Fase 3: agregados CRUD de presupuestos, GET /api/dashboard/resumen y GET /api/dashboard/estadisticas. Saldo histórico, ingresos y gastos mensuales, agrupación por categoría y progreso de cada presupuesto calculados en SQL Server. Prueba HTTPS ampliada y ejecutada: duplicado 409, aislamiento, gasto de 30 MXN, saldo -30 y avance de 30. Sin cambios de esquema. Siguiente tarea: endpoints del asistente.

Fase 3: incorporados AsistenteController, PrediccionService y PrediccionRepository, persistencia de predicciones y conversación, sugerencias y confirmación separada. Se reutilizó el esquema inicial. Pruebas: dotnet test pasó cinco casos, cuatro del pronóstico y uno de plantilla. integracion_https.py pasó con SQL Server: 30 MXN en noviembre, sin datos anteriores para estimar noviembre, estimación de 30 y sugerencia de 31.50 en diciembre. Rechazadas confirmación ausente, cifra alterada y duplicado. Chat aislado entre dos cuentas. docs/IA.md explica las decisiones y limitaciones. Pendiente validación móvil y proveedor opcional de la fase 6. Siguiente tarea: administración y seguridad de roles.

Fase 4: añadidos listado paginado de cuentas y cambio de rol solo para administradores. Configuración documentada para promover la primera cuenta de forma local. dotnet test pasó seis pruebas (una de plantilla), incluida expiración JWT. integracion_roles.py y regresión HTTPS pasaron sobre SQL Server: 403 a usuario, 401 tras cambiar rol, promoción/revocación y rechazo de autoedición. Sin migraciones nuevas. Pendiente cliente SecureStore y validación Android.

Fases 4 y 5: cliente Axios HTTPS con EXPO_PUBLIC_API_URL, token en SecureStore, restauración por auth/me, limpieza al cerrar sesión y tratamiento 401/403. Implementados bienvenida, registro, login, perfil, resumen de saldo e historial paginado; altas de categorías, ingresos y gastos, edición, recurrencia y borrado con confirmación. Se adaptaron las rutas de plantilla y se añadieron componentes de formulario. Sin migraciones ni cambios API en este bloque. npx tsc --noEmit pasó; npx eslint app lib components terminó con cero errores y una advertencia sobre axios.create. Expo export --platform android generó un bundle de 2.7 MB. No se instaló APK ni se verificó la sesión en un dispositivo. Faltan pantallas de gestión de categorías, presupuestos, estadísticas, chat, predicciones y administración, y validación extremo a extremo de Android. Próxima tarea: completar esas rutas y verificar navegación y formularios.
