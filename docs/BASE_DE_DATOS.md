# Base de datos de desarrollo
La migración Inicial define Usuarios, Categorias, Movimientos, Presupuestos, Predicciones y ConversacionesIA. Las claves compuestas impiden asociar movimientos, presupuestos o predicciones a categorías ajenas. La clave de categoría del movimiento también exige coincidencia de tipo. Los presupuestos y predicciones son únicos por usuario, categoría y periodo.

Desde backend, con ConnectionStrings__DefaultConnection configurada en el entorno para una base de desarrollo autorizada, ejecutar:

    dotnet tool restore
    dotnet ef database update --project MoneyTrack.Infrastructure --startup-project MoneyTrack.Api

El script equivalente está en database/scripts/001_inicial.sql. La generación del script se verificó localmente. Sigue pendiente ejecutarlo contra SQL Server y comprobar restricciones en el motor.

SQL Server Express 2022 LocalDB instalado. Instancia privada (localdb)\MoneyTrack, base MoneyTrack, autenticación integrada de Windows. En SSMS usar ese servidor y autenticación de Windows. En desarrollo la API recupera ConnectionStrings:DefaultConnection de dotnet user-secrets. La migración 20261123041800_Inicial quedó aplicada; una segunda ejecución de dotnet ef database update confirmó que la base estaba actualizada. Ejecutar database/seeds/verificar_demo.sql sobre MoneyTrack para reproducir los casos de prueba. El script inserta dos usuarios temporales, un gasto y un presupuesto, comprueba el rechazo de categoría ajena, importe negativo y presupuesto duplicado, y revierte la transacción. Resultado observado: cero usuarios al terminar. El cliente legado System.Data.SqlClient requirió Encrypt=False en su conexión local de prueba; EF usó su configuración independiente con Encrypt=True. Próxima tarea: implementar y probar registro, login y perfil contra esta instancia.
