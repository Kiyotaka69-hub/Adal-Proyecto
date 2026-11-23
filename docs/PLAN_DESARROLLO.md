# Plan de desarrollo
Cada fase exige registrar archivos, migraciones, rutas, pruebas, problemas, decisiones, pendientes y sincronización antes de avanzar.

| Fase | Entregable | Criterio de salida |
|---|---|---|
| 0 | Requisitos y plan | Revisar fuente, repositorio y entorno; versionar documentación. |
| 1 | Monorepositorio | Compilar solución .NET 8 y verificar TypeScript en Expo. |
| 2 | Esquema SQL Server | Aplicar migraciones a una base vacía y probar restricciones y seeds. |
| 3 | API | Verificar contratos de sesión, movimientos, categorías, presupuestos, dashboard y asistente con datos persistidos. |
| 4 | Seguridad | Probar aislamiento, roles, caducidad, BCrypt y errores. |
| 5 | Android | Recorrer las 15 pantallas, validaciones y navegación con accesibilidad. |
| 6 | Asistente | Verificar tendencia, meses faltantes, atípicos, privacidad y confirmación. |
| 7 | Integración | Reconciliar dashboard, presupuestos y gráficas después de editar y eliminar. |
| 8 | Calidad | Ejecutar xUnit, pruebas móviles, lint, TypeScript y CI. |
| 9 | Documentación | Reproducir instalación y contrastar informe con evidencia. |
| 10 | GitHub | Verificar rama remota, hashes y PR. |

## Decisiones de diseño previstas
Expo Router y Axios consumirán por HTTPS los DTO de la API. SecureStore custodiará el JWT. Los servicios derivarán el usuario del token. El repositorio filtrará por propietario incluso al consultar por clave primaria. Añadir propietario a categorías permite cumplir RF-08 y RF-12. El rol persistido distinguirá Usuario y Administrador. SQL Server conservará importes decimales y la API calculará totales. Las sugerencias requerirán una acción de confirmación independiente del chat.

## Dependencias y riesgos
Se necesitan Git, SDK .NET 8, Node y npm, una instancia SQL Server, emulador o dispositivo Android y credenciales GitHub con escritura para publicar. El pronóstico local permite validar todo el flujo sin gastos externos. Con historiales cortos se documentará la cobertura antes de mostrar estimaciones.
