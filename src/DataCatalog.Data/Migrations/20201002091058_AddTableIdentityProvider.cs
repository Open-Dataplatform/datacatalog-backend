using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DataCatalog.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddTableIdentityProvider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IdentityProvider",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    TenantId = table.Column<string>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProvider", x => x.Id);
                }
            );

            migrationBuilder.InsertData(
                table: "IdentityProvider",
                columns: new[] { "Id", "Name", "TenantId" },
                values: new object[,]
                {
                    { 
                        new Guid("75030760-f7f8-40d8-a1ab-718bcb7327b7"), "Azure AD", "f7619355-6c67-4100-9a78-1847f30742e2" 
                    }
                }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityProvider"
            );
        }
    }
}
