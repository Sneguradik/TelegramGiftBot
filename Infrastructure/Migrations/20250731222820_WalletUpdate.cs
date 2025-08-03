using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WalletUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TelegramClientWallets_ClientId",
                table: "TelegramClientWallets");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramClientWallets_ClientId",
                table: "TelegramClientWallets",
                column: "ClientId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TelegramClientWallets_ClientId",
                table: "TelegramClientWallets");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramClientWallets_ClientId",
                table: "TelegramClientWallets",
                column: "ClientId");
        }
    }
}
