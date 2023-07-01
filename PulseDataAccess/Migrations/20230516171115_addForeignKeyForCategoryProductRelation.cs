using Microsoft.Build.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PulseDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addForeignKeyForCategoryProductRelation : Migration
    {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "CategoryId",
				table: "Products",
				type: "int",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.UpdateData(
				table: "Products",
				keyColumn: "Id",
				keyValue: 1,
				column: "CategoryId",
				value: 1);

			migrationBuilder.UpdateData(
				table: "Products",
				keyColumn: "Id",
				keyValue: 2,
				column: "CategoryId",
				value: 2);

			migrationBuilder.CreateIndex(
				name: "IX_Products_CategoryId",
				table: "Products",
				column: "CategoryId");
		}

		
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Products_Category_CategoryId",
				table: "Products");
		}
	}
}
