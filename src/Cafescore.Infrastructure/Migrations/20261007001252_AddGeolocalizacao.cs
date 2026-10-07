using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cafescore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGeolocalizacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Clinicas",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Clinicas",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "OsmId",
                table: "Clinicas",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LatitudeCheckIn",
                table: "Avaliacoes",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LongitudeCheckIn",
                table: "Avaliacoes",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlFoto",
                table: "Avaliacoes",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clinicas_OsmId",
                table: "Clinicas",
                column: "OsmId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clinicas_OsmId",
                table: "Clinicas");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Clinicas");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Clinicas");

            migrationBuilder.DropColumn(
                name: "OsmId",
                table: "Clinicas");

            migrationBuilder.DropColumn(
                name: "LatitudeCheckIn",
                table: "Avaliacoes");

            migrationBuilder.DropColumn(
                name: "LongitudeCheckIn",
                table: "Avaliacoes");

            migrationBuilder.DropColumn(
                name: "UrlFoto",
                table: "Avaliacoes");
        }
    }
}
