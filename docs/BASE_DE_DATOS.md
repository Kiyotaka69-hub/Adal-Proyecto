# Base de datos de desarrollo
La migración Inicial define Usuarios, Categorias, Movimientos, Presupuestos, Predicciones y ConversacionesIA. Las claves compuestas impiden asociar movimientos, presupuestos o predicciones a categorías ajenas. La clave de categoría del movimiento también exige coincidencia de tipo. Los presupuestos y predicciones son únicos por usuario, categoría y periodo.

Desde backend, con ConnectionStrings__DefaultConnection configurada en el entorno para una base de desarrollo autorizada, ejecutar:

    dotnet tool restore
    dotnet ef database update --project MoneyTrack.Infrastructure --startup-project MoneyTrack.Api

El script equivalente está en database/scripts/001_inicial.sql. La generación del script se verificó localmente. Sigue pendiente ejecutarlo contra SQL Server y comprobar restricciones en el motor.
