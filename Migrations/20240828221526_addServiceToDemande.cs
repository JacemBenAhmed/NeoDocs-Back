using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestBurOrdAPI.Migrations
{
    /// <inheritdoc />
    public partial class addServiceToDemande : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "Demandes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Demandes_ServiceId",
                table: "Demandes",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Demandes_Services_ServiceId",
                table: "Demandes",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Demandes_Services_ServiceId",
                table: "Demandes");

            migrationBuilder.DropIndex(
                name: "IX_Demandes_ServiceId",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "Demandes");
        }
    }
}
