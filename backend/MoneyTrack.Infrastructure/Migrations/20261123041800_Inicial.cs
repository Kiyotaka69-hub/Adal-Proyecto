using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    correo = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    fecha_registro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    rol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.id_usuario);
                    table.CheckConstraint("CK_Usuarios_1", "rol IN ('Usuario','Administrador')");
                });

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    id_categoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    tipo = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.id_categoria);
                    table.UniqueConstraint("AK_Categorias_id_usuario_id_categoria", x => new { x.id_usuario, x.id_categoria });
                    table.UniqueConstraint("AK_Categorias_id_usuario_id_categoria_tipo", x => new { x.id_usuario, x.id_categoria, x.tipo });
                    table.CheckConstraint("CK_Categorias_1", "tipo IN ('Ingreso','Gasto')");
                    table.ForeignKey(
                        name: "FK_Categorias_Usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConversacionesIA",
                columns: table => new
                {
                    id_mensaje = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    rol = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    contenido = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversacionesIA", x => x.id_mensaje);
                    table.CheckConstraint("CK_ConversacionesIA_1", "rol IN ('Usuario','Asistente')");
                    table.ForeignKey(
                        name: "FK_ConversacionesIA_Usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Movimientos",
                columns: table => new
                {
                    id_movimiento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    id_categoria = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    tipo = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    es_recurrente = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimientos", x => x.id_movimiento);
                    table.CheckConstraint("CK_Movimientos_1", "cantidad > 0");
                    table.CheckConstraint("CK_Movimientos_2", "tipo IN ('Ingreso','Gasto')");
                    table.ForeignKey(
                        name: "FK_Movimientos_Categorias_id_usuario_id_categoria_tipo",
                        columns: x => new { x.id_usuario, x.id_categoria, x.tipo },
                        principalTable: "Categorias",
                        principalColumns: new[] { "id_usuario", "id_categoria", "tipo" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Movimientos_Usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Predicciones",
                columns: table => new
                {
                    id_prediccion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    id_categoria = table.Column<int>(type: "int", nullable: false),
                    mes = table.Column<int>(type: "int", nullable: false),
                    anio = table.Column<int>(type: "int", nullable: false),
                    monto_predicho = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    confianza = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: false),
                    metodo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    fecha_generada = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Predicciones", x => x.id_prediccion);
                    table.CheckConstraint("CK_Predicciones_1", "mes BETWEEN 1 AND 12");
                    table.CheckConstraint("CK_Predicciones_2", "anio BETWEEN 1 AND 9999");
                    table.CheckConstraint("CK_Predicciones_3", "monto_predicho >= 0");
                    table.CheckConstraint("CK_Predicciones_4", "confianza BETWEEN 0 AND 1");
                    table.ForeignKey(
                        name: "FK_Predicciones_Categorias_id_usuario_id_categoria",
                        columns: x => new { x.id_usuario, x.id_categoria },
                        principalTable: "Categorias",
                        principalColumns: new[] { "id_usuario", "id_categoria" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Predicciones_Usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Presupuestos",
                columns: table => new
                {
                    id_presupuesto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    id_categoria = table.Column<int>(type: "int", nullable: false),
                    limite = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    mes = table.Column<int>(type: "int", nullable: false),
                    anio = table.Column<int>(type: "int", nullable: false),
                    origen = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    monto_sugerido = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presupuestos", x => x.id_presupuesto);
                    table.CheckConstraint("CK_Presupuestos_1", "limite > 0");
                    table.CheckConstraint("CK_Presupuestos_2", "mes BETWEEN 1 AND 12");
                    table.CheckConstraint("CK_Presupuestos_3", "anio BETWEEN 1 AND 9999");
                    table.CheckConstraint("CK_Presupuestos_4", "origen IN ('Manual','IA')");
                    table.CheckConstraint("CK_Presupuestos_5", "monto_sugerido IS NULL OR monto_sugerido >= 0");
                    table.ForeignKey(
                        name: "FK_Presupuestos_Categorias_id_usuario_id_categoria",
                        columns: x => new { x.id_usuario, x.id_categoria },
                        principalTable: "Categorias",
                        principalColumns: new[] { "id_usuario", "id_categoria" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Presupuestos_Usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_id_usuario_id_categoria",
                table: "Categorias",
                columns: new[] { "id_usuario", "id_categoria" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_id_usuario_id_categoria_tipo",
                table: "Categorias",
                columns: new[] { "id_usuario", "id_categoria", "tipo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_id_usuario_nombre_tipo",
                table: "Categorias",
                columns: new[] { "id_usuario", "nombre", "tipo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConversacionesIA_id_usuario",
                table: "ConversacionesIA",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_id_usuario_fecha",
                table: "Movimientos",
                columns: new[] { "id_usuario", "fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_id_usuario_id_categoria_tipo",
                table: "Movimientos",
                columns: new[] { "id_usuario", "id_categoria", "tipo" });

            migrationBuilder.CreateIndex(
                name: "IX_Predicciones_id_usuario_id_categoria_mes_anio",
                table: "Predicciones",
                columns: new[] { "id_usuario", "id_categoria", "mes", "anio" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Presupuestos_id_usuario_id_categoria_mes_anio",
                table: "Presupuestos",
                columns: new[] { "id_usuario", "id_categoria", "mes", "anio" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_correo",
                table: "Usuarios",
                column: "correo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConversacionesIA");

            migrationBuilder.DropTable(
                name: "Movimientos");

            migrationBuilder.DropTable(
                name: "Predicciones");

            migrationBuilder.DropTable(
                name: "Presupuestos");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
