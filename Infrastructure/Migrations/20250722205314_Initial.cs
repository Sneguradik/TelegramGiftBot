using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TelegramClients",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramClients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TelegramClientWallets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramClientWallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelegramClientWallets_TelegramClients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "TelegramClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TelegramRecipients",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: true),
                    RecipientType = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramRecipients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelegramRecipients_TelegramClients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "TelegramClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentInvoices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WalletId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentInvoices_TelegramClientWallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "TelegramClientWallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WalletId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_TelegramClientWallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "TelegramClientWallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GiftInvoices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecipientId = table.Column<long>(type: "bigint", nullable: false),
                    MinPrice = table.Column<double>(type: "double precision", nullable: true),
                    MaxPrice = table.Column<double>(type: "double precision", nullable: true),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiftInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GiftInvoices_TelegramRecipients_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "TelegramRecipients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GiftTransactions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecipientId = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    GiftId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiftTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GiftTransactions_TelegramRecipients_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "TelegramRecipients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GiftInvoices_RecipientId",
                table: "GiftInvoices",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_GiftTransactions_RecipientId",
                table: "GiftTransactions",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentInvoices_WalletId",
                table: "PaymentInvoices",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_WalletId",
                table: "PaymentTransactions",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramClientWallets_ClientId",
                table: "TelegramClientWallets",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramRecipients_ClientId",
                table: "TelegramRecipients",
                column: "ClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GiftInvoices");

            migrationBuilder.DropTable(
                name: "GiftTransactions");

            migrationBuilder.DropTable(
                name: "PaymentInvoices");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "TelegramRecipients");

            migrationBuilder.DropTable(
                name: "TelegramClientWallets");

            migrationBuilder.DropTable(
                name: "TelegramClients");
        }
    }
}
