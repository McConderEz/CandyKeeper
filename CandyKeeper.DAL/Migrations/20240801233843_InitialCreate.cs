using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CandyKeeper.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Suppliers",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Cities",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHashed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrincipalId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id");
                });

            migrationBuilder.AddCheckConstraint(
                name: "StoreNumberConstrains",
                table: "Stores",
                sql: "StoreNumber >= 100000 And StoreNumber <= 999999");

            migrationBuilder.AddCheckConstraint(
                name: "YearOfOpenedConstraint",
                table: "Stores",
                sql: "YEAR(YearOfOpened) <= YEAR(GETDATE())");

            migrationBuilder.AddCheckConstraint(
                name: "PriceConstraint",
                table: "ProductForSales",
                sql: "Price >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "VolumeConstraint",
                table: "ProductForSales",
                sql: "Volume >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StoreId",
                table: "Users",
                column: "StoreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropCheckConstraint(
                name: "StoreNumberConstrains",
                table: "Stores");

            migrationBuilder.DropCheckConstraint(
                name: "YearOfOpenedConstraint",
                table: "Stores");

            migrationBuilder.DropCheckConstraint(
                name: "PriceConstraint",
                table: "ProductForSales");

            migrationBuilder.DropCheckConstraint(
                name: "VolumeConstraint",
                table: "ProductForSales");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Suppliers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);
        }
    }
}
