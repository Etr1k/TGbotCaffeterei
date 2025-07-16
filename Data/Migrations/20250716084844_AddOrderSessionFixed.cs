using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CoffeBotAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderSessionFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Создаём таблицу сессий
            migrationBuilder.CreateTable(
                name: "OrderSessions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartedAt = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderSessions", x => x.Id);
                });

            // 2. Добавляем новые столбцы
            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocalNumber",
                table: "Orders",
                nullable: false,
                defaultValue: 0);

            // 3. Создаём первую сессию
            migrationBuilder.Sql("INSERT INTO \"OrderSessions\" (\"StartedAt\", \"IsActive\") VALUES (NOW(), TRUE);");

            // 4. Обновляем все старые заказы — привязываем их к этой сессии
            migrationBuilder.Sql("UPDATE \"Orders\" SET \"SessionId\" = (SELECT \"Id\" FROM \"OrderSessions\" WHERE \"IsActive\" = TRUE LIMIT 1);");

            // 5. Сделать SessionId обязательным
            migrationBuilder.AlterColumn<int>(
                name: "SessionId",
                table: "Orders",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            // 6. Добавляем внешний ключ
            migrationBuilder.CreateIndex(
                name: "IX_Orders_SessionId",
                table: "Orders",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderSessions_SessionId",
                table: "Orders",
                column: "SessionId",
                principalTable: "OrderSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderSessions_SessionId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "OrderSessions");

            migrationBuilder.DropIndex(
                name: "IX_Orders_SessionId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "LocalNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Orders",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
