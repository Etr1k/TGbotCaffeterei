using Microsoft.EntityFrameworkCore.Migrations;

   namespace CoffeBotAPI.Migrations
   {
       public partial class ChangeUserIdToInteger : Migration
       {
           protected override void Up(MigrationBuilder migrationBuilder)
           {
               // Добавляем временный столбец UserIdTemp
               migrationBuilder.AddColumn<int>(
                   name: "UserIdTemp",
                   table: "Orders",
                   type: "integer",
                   nullable: false,
                   defaultValue: 0);

               // Переносим данные из UserId (text) в UserIdTemp (integer)
               migrationBuilder.Sql(
                   @"UPDATE ""Orders"" 
                     SET ""UserIdTemp"" = CAST(""UserId"" AS INTEGER);");

               // Удаляем старый столбец UserId
               migrationBuilder.DropColumn(
                   name: "UserId",
                   table: "Orders");

               // Переименовываем UserIdTemp в UserId
               migrationBuilder.RenameColumn(
                   name: "UserIdTemp",
                   table: "Orders",
                   newName: "UserId");
           }

           protected override void Down(MigrationBuilder migrationBuilder)
           {
               // Добавляем столбец UserId (text)
               migrationBuilder.AddColumn<string>(
                   name: "UserId",
                   table: "Orders",
                   type: "text",
                   nullable: false,
                   defaultValue: "");

               // Переносим данные обратно
               migrationBuilder.Sql(
                   @"UPDATE ""Orders"" 
                     SET ""UserId"" = CAST(""UserIdTemp"" AS TEXT);");

               // Удаляем временный столбец
               migrationBuilder.DropColumn(
                   name: "UserIdTemp",
                   table: "Orders");

               // Переименовываем UserIdTemp в UserId
               migrationBuilder.RenameColumn(
                   name: "UserId",
                   table: "Orders",
                   newName: "UserIdTemp");
           }
       }
   }