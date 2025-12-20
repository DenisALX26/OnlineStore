using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStoreApp.Migrations
{
    /// <inheritdoc />
    public partial class NewColumnForProposalTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Proposals",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "Proposals");
        }
    }
}
