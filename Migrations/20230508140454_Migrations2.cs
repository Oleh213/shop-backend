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
            migrationBuilder.DropColumn(
                name: "Asap",
                table: "deliveryOptions");

            migrationBuilder.DropColumn(
                name: "Delivery",
                table: "deliveryOptions");

            migrationBuilder.DropColumn(
                name: "Domofon",
                table: "deliveryOptions");

            migrationBuilder.DropColumn(
                name: "Entrance",
                table: "deliveryOptions");

            migrationBuilder.DropColumn(
                name: "Flat",
                table: "deliveryOptions");

            migrationBuilder.DropColumn(
                name: "OnTime",
                table: "deliveryOptions");

            migrationBuilder.DropColumn(
                name: "PicUp",
                table: "deliveryOptions");

            migrationBuilder.RenameColumn(
                name: "Street",
                table: "deliveryOptions",
                newName: "Longitude");

            migrationBuilder.RenameColumn(
                name: "House",
                table: "deliveryOptions",
                newName: "Latitude");

            migrationBuilder.RenameColumn(
                name: "Flour",
                table: "deliveryOptions",
                newName: "Address");

            migrationBuilder.AddColumn<string>(
                name: "OrderMessage",
                table: "orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DeliveryTimeOptions",
                table: "deliveryOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryType",
                table: "deliveryOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderMessage",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "DeliveryTimeOptions",
                table: "deliveryOptions");

            migrationBuilder.DropColumn(
                name: "DeliveryType",
                table: "deliveryOptions");

            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "deliveryOptions",
                newName: "Street");

            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "deliveryOptions",
                newName: "House");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "deliveryOptions",
                newName: "Flour");

            migrationBuilder.AddColumn<bool>(
                name: "Asap",
                table: "deliveryOptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Delivery",
                table: "deliveryOptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Domofon",
                table: "deliveryOptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Entrance",
                table: "deliveryOptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Flat",
                table: "deliveryOptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "OnTime",
                table: "deliveryOptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PicUp",
                table: "deliveryOptions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
