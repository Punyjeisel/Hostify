using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hostify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_properties_User_HostId",
                table: "properties");

            migrationBuilder.DropForeignKey(
                name: "FK_reservations_User_GuestId",
                table: "reservations");

            migrationBuilder.CreateTable(
                name: "userRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userRoles", x => new { x.UserId, x.Role });
                    table.ForeignKey(
                        name: "FK_userRoles_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_properties_User_HostId",
                table: "properties",
                column: "HostId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_reservations_User_GuestId",
                table: "reservations",
                column: "GuestId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_properties_User_HostId",
                table: "properties");

            migrationBuilder.DropForeignKey(
                name: "FK_reservations_User_GuestId",
                table: "reservations");

            migrationBuilder.DropTable(
                name: "userRoles");

            migrationBuilder.AddForeignKey(
                name: "FK_properties_User_HostId",
                table: "properties",
                column: "HostId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reservations_User_GuestId",
                table: "reservations",
                column: "GuestId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
