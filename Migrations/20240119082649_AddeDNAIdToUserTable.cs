using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace biometricService.Migrations
{
    /// <inheritdoc />
    public partial class AddeDNAIdToUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "eDNAId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 100000);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "eDNAId",
                table: "Users");
        }
    }
}
