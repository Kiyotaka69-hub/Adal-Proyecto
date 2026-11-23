# Requisitos y aceptación
Fuente primaria: MoneyTrack – Presentación + Asistente IA, primera entrega, 11 de noviembre de 2026. Estado inicial de todos los requisitos: pendiente de implementación y prueba.

| ID | Evidencia exigida |
|---|---|
| RF-01/02 | Registrar una cuenta, iniciar sesión, consultar el perfil, cerrar y comprobar la eliminación del token. |
| RF-03 | Reconciliar el saldo con ingresos menos gastos del usuario. |
| RF-04/05 | Registrar un ingreso y un gasto positivos en su categoría y comprobar su persistencia. |
| RF-06/07 | Editar y eliminar movimientos, filtrar historial y comprobar totales actualizados. |
| RF-08 | Crear, consultar, editar y eliminar categorías propias respetando las referencias existentes. |
| RF-09 | Crear un límite mensual y comprobar el progreso y la unicidad por categoría, usuario y periodo. |
| RF-10/11 | Reconciliar gráficas y resumen por categoría con los movimientos del periodo. |
| RF-12 | Probar con dos cuentas que ninguna puede leer ni modificar datos de la otra. |
| RF-13 | Obtener pronóstico por categoría con método, fecha y explicación de confianza. |
| RF-14 | Solicitar una sugerencia, comprobar que el presupuesto sigue intacto y aplicarla tras confirmación. |
| RNF-01 | Verificar hash BCrypt y rechazo de contraseña incorrecta. |
| RNF-02 | Rechazar JWT ausente, alterado o vencido en operaciones privadas. |
| RNF-03 | Medir tiempos por ruta con volumen y entorno registrados. Umbral del PDF inferior a tres segundos. |
| RNF-04 | Completar registro, captura y consulta en Android con validación y mensajes legibles. |
| RNF-05 | Verificar dependencias Controller → Service → Repository y módulos separados. |
| RNF-06 | Instalar y ejecutar los flujos principales en Android. |
| RNF-07 | Probar FK, CHECK, unicidad y precisión decimal en SQL Server. |
| RNF-08 | Inspeccionar el payload del proveedor simulado y verificar que solo contiene agregados por categoría y periodo. |

El documento agrupa algunos identificadores y esta matriz conserva esas agrupaciones. El encargo amplía la entrega con Expo Router, gestión administrativa, CI, documentación y pruebas. Quedan fuera bancos, transferencias, pagos, conversión de monedas e inversiones.
