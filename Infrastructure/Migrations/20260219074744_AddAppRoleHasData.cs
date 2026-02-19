using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAppRoleHasData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Conditionally add CartId column only if it does not already exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'Purchases' AND COLUMN_NAME = 'CartId'
                )
                BEGIN
                    ALTER TABLE [Purchases] ADD [CartId] int NOT NULL DEFAULT 0;
                END
            ");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1, "dc27fb53-1fa0-4563-bded-61924542cacd", "Admin", "ADMIN" },
                    { 2, "a57bd5fd-d5cd-44b6-8f54-72282e3a20fe", "UserApp", "USERAPP" }
                });

            // Conditionally create index only if it does not already exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE name = 'IX_Purchases_CartId'
                      AND object_id = OBJECT_ID('Purchases')
                )
                BEGIN
                    CREATE INDEX [IX_Purchases_CartId] ON [Purchases] ([CartId]);
                END
            ");

            // Conditionally add foreign key only if it does not already exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.foreign_keys
                    WHERE name = 'FK_Purchases_Carts_CartId'
                )
                BEGIN
                    ALTER TABLE [Purchases] ADD CONSTRAINT [FK_Purchases_Carts_CartId]
                    FOREIGN KEY ([CartId]) REFERENCES [Carts] ([Id]) ON DELETE NO ACTION
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Conditionally drop foreign key only if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.foreign_keys
                    WHERE name = 'FK_Purchases_Carts_CartId'
                )
                BEGIN
                    ALTER TABLE [Purchases] DROP CONSTRAINT [FK_Purchases_Carts_CartId];
                END
            ");

            // Conditionally drop index only if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE name = 'IX_Purchases_CartId'
                      AND object_id = OBJECT_ID('Purchases')
                )
                BEGIN
                    DROP INDEX [IX_Purchases_CartId] ON [Purchases];
                END
            ");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2);

            // Conditionally drop column only if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'Purchases' AND COLUMN_NAME = 'CartId'
                )
                BEGIN
                    ALTER TABLE [Purchases] DROP COLUMN [CartId];
                END
            ");
        }
    }
}
