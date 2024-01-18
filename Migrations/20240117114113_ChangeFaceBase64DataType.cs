using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace biometricService.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFaceBase64DataType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FaceBase64",
                table: "FaceData",
                type: "varchar(max)",
                unicode: false,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FaceBase64",
                table: "FaceData",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false);
        }
    }
}
