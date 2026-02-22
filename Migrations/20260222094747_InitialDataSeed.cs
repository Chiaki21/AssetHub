using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetHub.Migrations
{
    /// <inheritdoc />
    public partial class InitialDataSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "DateAdded",
                value: new DateTime(2026, 2, 22, 17, 47, 47, 502, DateTimeKind.Local).AddTicks(900));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "DateAdded",
                value: new DateTime(2026, 2, 22, 17, 47, 47, 502, DateTimeKind.Local).AddTicks(913));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Email", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 22, 17, 47, 47, 501, DateTimeKind.Local).AddTicks(9943), "testuser@assethub.com", "$2a$11$xtmgAtVQidDZS/97ueb9Fuw1rNcPVlyDn1KJkesBU9ggXAvVvuZPy" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "DateAdded",
                value: new DateTime(2026, 2, 22, 17, 43, 17, 44, DateTimeKind.Local).AddTicks(3328));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "DateAdded",
                value: new DateTime(2026, 2, 22, 17, 43, 17, 44, DateTimeKind.Local).AddTicks(3336));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Email", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 22, 17, 43, 17, 44, DateTimeKind.Local).AddTicks(2489), "admin@assethub.com", "$2a$11$J4lTxWQvwdl12jZAsqaX7udyN/Ri9vYp.XxVtXQefXDmnc43aLEEi" });
        }
    }
}
