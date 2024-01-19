using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace biometricService.Migrations
{
    /// <inheritdoc />
    public partial class FixeDNAId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "eDNAId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValueSql: "100001",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 100000);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "eDNAId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 100000,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "100001");
        }
    }
}
