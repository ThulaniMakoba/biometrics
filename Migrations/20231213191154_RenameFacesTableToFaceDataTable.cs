using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace biometricService.Migrations
{
    /// <inheritdoc />
    public partial class RenameFacesTableToFaceDataTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Faces_Users_UserId",
                table: "Faces");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Faces",
                table: "Faces");

            migrationBuilder.RenameTable(
                name: "Faces",
                newName: "FaceData");

            migrationBuilder.RenameIndex(
                name: "IX_Faces_UserId",
                table: "FaceData",
                newName: "IX_FaceData_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FaceData",
                table: "FaceData",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FaceData_Users_UserId",
                table: "FaceData",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FaceData_Users_UserId",
                table: "FaceData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FaceData",
                table: "FaceData");

            migrationBuilder.RenameTable(
                name: "FaceData",
                newName: "Faces");

            migrationBuilder.RenameIndex(
                name: "IX_FaceData_UserId",
                table: "Faces",
                newName: "IX_Faces_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Faces",
                table: "Faces",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Faces_Users_UserId",
                table: "Faces",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
