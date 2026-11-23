# Continuidad
MoneyTrack combina Android Expo, API .NET 8 por capas, SQL Server y predicción local. Consultar PLAN_DESARROLLO.md y REQUISITOS.md para los criterios. Fase 0 en curso. Última tarea: inspección y planificación. Próximo paso: habilitar Git, configurar el remoto https://github.com/Kiyotaka69-hub/Adal-Proyecto.git y verificar de nuevo git ls-remote antes de crear la rama de trabajo. Ninguna rama local o revisión publicada todavía.

Variables previstas: ConnectionStrings__DefaultConnection, Jwt__Key, Jwt__Issuer, Jwt__Audience, EXPO_PUBLIC_API_URL. La clave opcional del LLM residirá únicamente en backend. Los comandos de compilación y ejecución deberán validarse al crear los proyectos. El servicio SQL y Android requieren aprovisionamiento.

Actualización: Git instalado, rama local desarrollo/moneytrack, planificación en d46ed52. Push pendiente de autenticación. Fase 1 iniciada. Verificar instalación del SDK .NET y generar los proyectos.

Fase 1 validada. Desde backend ejecutar dotnet build y dotnet test. Desde frontend npm ci, npx tsc --noEmit y npx eslint app components constants. Próxima tarea: crear entidades y migraciones, y habilitar SQL Server de prueba. Rama desarrollo/moneytrack publicada.
