using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GolTime.Migrations
{
    /// <inheritdoc />
    public partial class IngresoDeTablasAndSeders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EstadoReserva",
                table: "Reservaciones",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "EstadoPago",
                table: "Reservaciones",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20);

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "Id", "CreatedAt", "Nombre", "Numero", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 7, 27, 0, 29, 44, 889, DateTimeKind.Utc).AddTicks(922), "Juan Pérez", "7534-4312", new DateTime(2026, 7, 27, 0, 29, 44, 889, DateTimeKind.Utc).AddTicks(1584) },
                    { 2, new DateTime(2026, 7, 27, 0, 29, 44, 889, DateTimeKind.Utc).AddTicks(2218), "María López", "7352-9702", new DateTime(2026, 7, 27, 0, 29, 44, 889, DateTimeKind.Utc).AddTicks(2219) },
                    { 3, new DateTime(2026, 7, 27, 0, 29, 44, 889, DateTimeKind.Utc).AddTicks(2221), "Carlos Hernández", "754-0232", new DateTime(2026, 7, 27, 0, 29, 44, 889, DateTimeKind.Utc).AddTicks(2221) }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Nombre", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 7, 27, 0, 29, 44, 886, DateTimeKind.Utc).AddTicks(611), "Administrador", new DateTime(2026, 7, 27, 0, 29, 44, 886, DateTimeKind.Utc).AddTicks(1290) },
                    { 2, new DateTime(2026, 7, 27, 0, 29, 44, 886, DateTimeKind.Utc).AddTicks(2244), "Empleado", new DateTime(2026, 7, 27, 0, 29, 44, 886, DateTimeKind.Utc).AddTicks(2245) }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Clave", "Correo", "CreatedAt", "Estado", "Nombre", "RolId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Admin123*", "admin@goltime.com", new DateTime(2026, 7, 27, 0, 29, 44, 890, DateTimeKind.Utc).AddTicks(1173), true, "Administrador", 1, new DateTime(2026, 7, 27, 0, 29, 44, 890, DateTimeKind.Utc).AddTicks(2257) },
                    { 2, "Empleado123*", "empleado@goltime.com", new DateTime(2026, 7, 27, 0, 29, 44, 890, DateTimeKind.Utc).AddTicks(2887), true, "Empleado", 2, new DateTime(2026, 7, 27, 0, 29, 44, 890, DateTimeKind.Utc).AddTicks(2888) }
                });

            migrationBuilder.InsertData(
                table: "Reservaciones",
                columns: new[] { "Id", "ClientId", "CreatedAt", "EstadoPago", "EstadoReserva", "Fecha", "HoraFin", "HoraInicio", "UpdatedAt", "UserId" },
                values: new object[] { 1, 1, new DateTime(2026, 7, 27, 0, 29, 44, 888, DateTimeKind.Utc).AddTicks(2197), "Pendiente", "Activa", new DateOnly(2026, 7, 27), new TimeOnly(10, 0, 0), new TimeOnly(8, 0, 0), new DateTime(2026, 7, 27, 0, 29, 44, 888, DateTimeKind.Utc).AddTicks(2821), 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Reservaciones",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "EstadoReserva",
                table: "Reservaciones",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "EstadoPago",
                table: "Reservaciones",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
