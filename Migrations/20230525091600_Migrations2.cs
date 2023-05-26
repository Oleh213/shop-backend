using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sushi_backend.Migrations
{
    /// <inheritdoc />
    public partial class Migrations2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "magazineSettings",
                columns: table => new
                {
                    MagazineSettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<bool>(type: "bit", nullable: false),
                    OnePlusOneAction = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_magazineSettings", x => x.MagazineSettingsId);
                });

            migrationBuilder.CreateTable(
                name: "timeLines",
                columns: table => new
                {
                    TimeLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    From = table.Column<DateTime>(type: "datetime2", nullable: false),
                    To = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeConfig = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsOpen = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_timeLines", x => x.TimeLineId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "magazineSettings");

            migrationBuilder.DropTable(
                name: "timeLines");
        }
    }
}
