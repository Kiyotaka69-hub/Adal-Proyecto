using Microsoft.EntityFrameworkCore;
using MoneyTrack.Domain;
using System.Text.RegularExpressions;

namespace MoneyTrack.Infrastructure;

public sealed class MoneyTrackContext(DbContextOptions<MoneyTrackContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Movimiento> Movimientos => Set<Movimiento>();
    public DbSet<Presupuesto> Presupuestos => Set<Presupuesto>();
    public DbSet<Prediccion> Predicciones => Set<Prediccion>();
    public DbSet<MensajeIA> ConversacionesIA => Set<MensajeIA>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios", table =>
            {
                table.HasCheckConstraint("CK_Usuarios_1", "rol IN ('Usuario','Administrador')");
            });
            entity.HasKey(x => x.IdUsuario);
            entity.Property(x => x.Nombre).HasMaxLength(120);
            entity.Property(x => x.Correo).HasMaxLength(254);
            entity.Property(x => x.PasswordHash).HasMaxLength(100);
            entity.Property(x => x.Rol).HasMaxLength(20);
            entity.HasIndex(x => new { x.Correo }).IsUnique();



        });

        model.Entity<Categoria>(entity =>
        {
            entity.ToTable("Categorias", table =>
            {
                table.HasCheckConstraint("CK_Categorias_1", "tipo IN ('Ingreso','Gasto')");
            });
            entity.HasKey(x => x.IdCategoria);
            entity.Property(x => x.Nombre).HasMaxLength(100);
            entity.Property(x => x.Tipo).HasMaxLength(7);
            entity.HasIndex(x => new { x.IdUsuario, x.IdCategoria }).IsUnique();
            entity.HasIndex(x => new { x.IdUsuario, x.IdCategoria, x.Tipo }).IsUnique();
            entity.HasIndex(x => new { x.IdUsuario, x.Nombre, x.Tipo }).IsUnique();
            entity.HasOne<Usuario>().WithMany().HasForeignKey(x => x.IdUsuario).OnDelete(DeleteBehavior.Restrict);


        });

        model.Entity<Movimiento>(entity =>
        {
            entity.ToTable("Movimientos", table =>
            {
                table.HasCheckConstraint("CK_Movimientos_1", "cantidad > 0");
                table.HasCheckConstraint("CK_Movimientos_2", "tipo IN ('Ingreso','Gasto')");
            });
            entity.HasKey(x => x.IdMovimiento);
            entity.Property(x => x.Tipo).HasMaxLength(7);
            entity.Property(x => x.Descripcion).HasMaxLength(500);
            entity.Property(x => x.Cantidad).HasPrecision(12, 2);

            entity.HasOne<Usuario>().WithMany().HasForeignKey(x => x.IdUsuario).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Categoria>().WithMany()
                .HasForeignKey(x => new { x.IdUsuario, x.IdCategoria, x.Tipo })
                .HasPrincipalKey(x => new { x.IdUsuario, x.IdCategoria, x.Tipo })
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.IdUsuario, x.Fecha });
        });

        model.Entity<Presupuesto>(entity =>
        {
            entity.ToTable("Presupuestos", table =>
            {
                table.HasCheckConstraint("CK_Presupuestos_1", "limite > 0");
                table.HasCheckConstraint("CK_Presupuestos_2", "mes BETWEEN 1 AND 12");
                table.HasCheckConstraint("CK_Presupuestos_3", "anio BETWEEN 1 AND 9999");
                table.HasCheckConstraint("CK_Presupuestos_4", "origen IN ('Manual','IA')");
                table.HasCheckConstraint("CK_Presupuestos_5", "monto_sugerido IS NULL OR monto_sugerido >= 0");
            });
            entity.HasKey(x => x.IdPresupuesto);
            entity.Property(x => x.Origen).HasMaxLength(6);
            entity.Property(x => x.Limite).HasPrecision(12, 2);
            entity.Property(x => x.MontoSugerido).HasPrecision(12, 2);
            entity.HasIndex(x => new { x.IdUsuario, x.IdCategoria, x.Mes, x.Anio }).IsUnique();
            entity.HasOne<Usuario>().WithMany().HasForeignKey(x => x.IdUsuario).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Categoria>().WithMany()
                .HasForeignKey(x => new { x.IdUsuario, x.IdCategoria })
                .HasPrincipalKey(x => new { x.IdUsuario, x.IdCategoria })
                .OnDelete(DeleteBehavior.Restrict);

        });

        model.Entity<Prediccion>(entity =>
        {
            entity.ToTable("Predicciones", table =>
            {
                table.HasCheckConstraint("CK_Predicciones_1", "mes BETWEEN 1 AND 12");
                table.HasCheckConstraint("CK_Predicciones_2", "anio BETWEEN 1 AND 9999");
                table.HasCheckConstraint("CK_Predicciones_3", "monto_predicho >= 0");
                table.HasCheckConstraint("CK_Predicciones_4", "confianza BETWEEN 0 AND 1");
            });
            entity.HasKey(x => x.IdPrediccion);
            entity.Property(x => x.Metodo).HasMaxLength(100);
            entity.Property(x => x.MontoPredicho).HasPrecision(12, 2);
            entity.Property(x => x.Confianza).HasPrecision(5, 4);
            entity.HasIndex(x => new { x.IdUsuario, x.IdCategoria, x.Mes, x.Anio }).IsUnique();
            entity.HasOne<Usuario>().WithMany().HasForeignKey(x => x.IdUsuario).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Categoria>().WithMany()
                .HasForeignKey(x => new { x.IdUsuario, x.IdCategoria })
                .HasPrincipalKey(x => new { x.IdUsuario, x.IdCategoria })
                .OnDelete(DeleteBehavior.Restrict);

        });

        model.Entity<MensajeIA>(entity =>
        {
            entity.ToTable("ConversacionesIA", table =>
            {
                table.HasCheckConstraint("CK_ConversacionesIA_1", "rol IN ('Usuario','Asistente')");
            });
            entity.HasKey(x => x.IdMensaje);
            entity.Property(x => x.Rol).HasMaxLength(12);
            entity.Property(x => x.Contenido).HasMaxLength(4000);

            entity.HasOne<Usuario>().WithMany().HasForeignKey(x => x.IdUsuario).OnDelete(DeleteBehavior.Restrict);


        });

        foreach (var entity in model.Model.GetEntityTypes())
        foreach (var property in entity.GetProperties())
            property.SetColumnName(Regex.Replace(property.Name, "(?<=[a-z])([A-Z])", "_$1").ToLowerInvariant());
    }
}
