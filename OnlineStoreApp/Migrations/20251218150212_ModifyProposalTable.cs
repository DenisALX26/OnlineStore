using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStoreApp.Migrations
{
    /// <inheritdoc />
    public partial class ModifyProposalTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Proposals",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Proposals",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Proposals",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "Proposals",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Proposals");
        }
    }
}
