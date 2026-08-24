using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRG.EVA01.SeaBattle.Migrations
{
    /// <inheritdoc />
    public partial class Phase2EntityRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartedPlayingOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[GameLogs]', N'U') IS NULL
BEGIN
    CREATE TABLE [GameLogs] (
        [Id] int NOT NULL IDENTITY,
        [GameId] int NOT NULL,
        [PlayerName] nvarchar(max) NOT NULL,
        [LocationLetter] nvarchar(max) NOT NULL,
        [LocationNumber] nvarchar(max) NOT NULL,
        [Result] nvarchar(max) NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        CONSTRAINT [PK_GameLogs] PRIMARY KEY ([Id])
    );
END
ELSE
BEGIN
    UPDATE [GameLogs]
    SET [PlayerName] = ISNULL([PlayerName], ''),
        [LocationLetter] = ISNULL([LocationLetter], ''),
        [Result] = ISNULL([Result], ''),
        [LocationNumber] = CONVERT(nvarchar(max), [LocationNumber]);

    ALTER TABLE [GameLogs] ALTER COLUMN [PlayerName] nvarchar(max) NOT NULL;
    ALTER TABLE [GameLogs] ALTER COLUMN [LocationLetter] nvarchar(max) NOT NULL;
    ALTER TABLE [GameLogs] ALTER COLUMN [LocationNumber] nvarchar(max) NOT NULL;
    ALTER TABLE [GameLogs] ALTER COLUMN [Result] nvarchar(max) NOT NULL;
END
");

            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM [GameLogs])
BEGIN
    DELETE FROM [GameLogs];
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_GameLogs_Games_GameId')
BEGIN
    ALTER TABLE [GameLogs]
    ADD CONSTRAINT [FK_GameLogs_Games_GameId]
    FOREIGN KEY ([GameId]) REFERENCES [Games]([Id]) ON DELETE CASCADE;
END
");

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letter = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Boats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Boats_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Boats_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Boats_GameId",
                table: "Boats",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Boats_LocationId",
                table: "Boats",
                column: "LocationId",
                unique: true);

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_GameLogs_GameId' AND object_id = OBJECT_ID(N'[GameLogs]'))
BEGIN
    CREATE INDEX [IX_GameLogs_GameId] ON [GameLogs] ([GameId]);
END
");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_GameId_Letter_Number",
                table: "Locations",
                columns: new[] { "GameId", "Letter", "Number" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Boats");

            migrationBuilder.DropTable(
                name: "GameLogs");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
