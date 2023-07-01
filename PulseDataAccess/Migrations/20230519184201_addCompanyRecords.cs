using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PulseDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addCompanyRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Companies",
                table: "Companies");

            migrationBuilder.RenameTable(
                name: "Companies",
                newName: "Company");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Company",
                table: "Company",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Company",
                columns: new[] { "Id", "City", "Name", "PhoneNumber", "PostalCode", "State", "StreetAddress" },
                values: new object[,]
                {
                    { 1, "New York", "Tech Solution", "212-555-1212", "10001", "NY", "123 Main St" },
                    { 2, "New York", "Pulse Inc.", "212-555-1212", "10001", "NY", "123 Tech St" },
                    { 3, "New York", "Zloy Inc.", "212-555-1212", "10001", "NY", "999 Work St" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Company",
                table: "Company");

            migrationBuilder.DeleteData(
                table: "Company",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Company",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Company",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.RenameTable(
                name: "Company",
                newName: "Companies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Companies",
                table: "Companies",
                column: "Id");
        }
    }
}
