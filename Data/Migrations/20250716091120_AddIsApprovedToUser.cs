using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeBotAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsApprovedToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAproved",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAproved",
                table: "Users");
        }
    }
}
