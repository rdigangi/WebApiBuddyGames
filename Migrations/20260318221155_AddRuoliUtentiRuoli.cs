using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApiBuddyGames.Migrations
{
    /// <inheritdoc />
    public partial class AddRuoliUtentiRuoli : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ruoli",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'1', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descrizione = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    DataCreazione = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    DataCancellazione = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ruoli", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "utenti_ruoli",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'1', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UtenteId = table.Column<int>(type: "integer", nullable: false),
                    RuoloId = table.Column<int>(type: "integer", nullable: false),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    DataCreazione = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    DataCancellazione = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_utenti_ruoli", x => x.Id);
                    table.ForeignKey(
                        name: "FK_utenti_ruoli_ruoli_RuoloId",
                        column: x => x.RuoloId,
                        principalTable: "ruoli",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_utenti_ruoli_utenti_UtenteId",
                        column: x => x.UtenteId,
                        principalTable: "utenti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ruoli",
                columns: new[] { "Id", "Attivo", "DataCancellazione", "DataCreazione", "Descrizione" },
                values: new object[,]
                {
                    { 1, true, null, new DateTime(2026, 3, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Amministratore" },
                    { 2, true, null, new DateTime(2026, 3, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Utente" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ruoli_Descrizione",
                table: "ruoli",
                column: "Descrizione",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_utenti_ruoli_RuoloId",
                table: "utenti_ruoli",
                column: "RuoloId");

            migrationBuilder.CreateIndex(
                name: "IX_utenti_ruoli_UtenteId_RuoloId",
                table: "utenti_ruoli",
                columns: new[] { "UtenteId", "RuoloId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "utenti_ruoli");

            migrationBuilder.DropTable(
                name: "ruoli");
        }
    }
}
