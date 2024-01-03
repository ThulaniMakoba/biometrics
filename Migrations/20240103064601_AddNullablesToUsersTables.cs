using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace biometricService.Migrations
{
    /// <inheritdoc />
    public partial class AddNullablesToUsersTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WindowsProfileId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "ComputerSID",
                table: "Users",
                newName: "ComputerMotherboardSerialNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ComputerMotherboardSerialNumber",
                table: "Users",
                newName: "ComputerSID");

            migrationBuilder.AddColumn<Guid>(
                name: "WindowsProfileId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
