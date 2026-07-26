using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CurrencyExchange.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                    table.UniqueConstraint("AK_Currencies_CurrencyCode", x => x.CurrencyCode);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MainCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    MoneyCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyRates", x => x.Id);
                    table.CheckConstraint("CK_CurrencyRates_Rate_Positive", "[Rate] > 0");
                    table.ForeignKey(
                        name: "FK_CurrencyRates_Currencies_MainCurrency",
                        column: x => x.MainCurrency,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CurrencyRates_Currencies_MoneyCurrency",
                        column: x => x.MoneyCurrency,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "CurrencyCode", "DisplayName" },
                values: new object[,]
                {
                    { new Guid("227c7742-7189-4320-b1a1-baefdf76bda9"), "USD", "United States dollar" },
                    { new Guid("44c0f848-1a58-449f-9f11-7978204a831f"), "GBP", "Pound sterling" },
                    { new Guid("4fdc8575-c76c-45c5-bdf3-cbdea54ee781"), "SEK", "Swedish krona" },
                    { new Guid("61400521-7945-41f9-99db-c5f9ffd3c4d7"), "NOK", "Norwegian krone" },
                    { new Guid("84878f30-8af2-420b-b6e5-fb5be2059039"), "JPY", "Japanese yen" },
                    { new Guid("8ba142c0-f452-42c3-849d-de409f099951"), "CHF", "Swiss franc" },
                    { new Guid("b26f0b71-b5ff-4239-a01b-31add647c4bf"), "LTU", "Lithuanian litas" },
                    { new Guid("dd4ce93c-e72b-4000-ac54-9c2fc459d4c1"), "EUR", "Euro" },
                    { new Guid("df732d6b-34c3-4322-a75d-b946f5835e6f"), "DKK", "Danish krone" }
                });

            migrationBuilder.InsertData(
                table: "CurrencyRates",
                columns: new[] { "Id", "MainCurrency", "MoneyCurrency", "Rate" },
                values: new object[,]
                {
                    { new Guid("5eebddfe-a5c6-4b25-916d-0a7609667f7e"), "EUR", "DKK", 7.4394m },
                    { new Guid("773135cd-a1b3-4f03-b1ab-e38d5253943e"), "GBP", "DKK", 8.5285m },
                    { new Guid("7e3c6e84-662b-44ff-90dc-553e445bd39b"), "LTU", "JPY", 3.4528m },
                    { new Guid("97a2ed34-b0ab-45be-a7c2-c365ebc6adbb"), "SEK", "DKK", 0.7610m },
                    { new Guid("a8764ce9-7a57-4e61-815d-d74d1fbe891a"), "NOK", "DKK", 0.7840m },
                    { new Guid("b2636d1a-f60c-4294-9b23-12c32f075c5c"), "USD", "DKK", 6.6311m },
                    { new Guid("b85f11fe-cd8f-4f88-b436-b56920a0dfed"), "CHF", "DKK", 6.8358m },
                    { new Guid("c1d567e5-8445-4eb5-89fe-066f34ae7bf0"), "JPY", "DKK", 0.059740m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyRates_MainCurrency",
                table: "CurrencyRates",
                column: "MainCurrency");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyRates_MoneyCurrency",
                table: "CurrencyRates",
                column: "MoneyCurrency");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyRates");

            migrationBuilder.DropTable(
                name: "Currencies");
        }
    }
}
