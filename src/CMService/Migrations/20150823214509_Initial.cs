using System.Collections.Generic;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Data.Entity.Relational.Migrations.Builders;
using Microsoft.Data.Entity.Relational.Migrations.Operations;

namespace CMService.Migrations
{
    public partial class Initial : Migration
    {
        public override void Up(MigrationBuilder migration)
        {
            migration.CreateSequence(
                name: "DefaultSequence",
                type: "bigint",
                startWith: 1L,
                incrementBy: 10);
            migration.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column(type: "int", nullable: false),
                    AddressLine1 = table.Column(type: "nvarchar(max)", nullable: true),
                    Category = table.Column(type: "nvarchar(max)", nullable: true),
                    Country = table.Column(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column(type: "datetime2", nullable: false),
                    Gender = table.Column(type: "nvarchar(max)", nullable: true),
                    HouseNumber = table.Column(type: "int", nullable: false),
                    Name = table.Column(type: "nvarchar(max)", nullable: true),
                    State = table.Column(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });
            migration.CreateTable(
                name: "CustomerUpdate",
                columns: table => new
                {
                    Id = table.Column(type: "int", nullable: false),
                    CustomerId = table.Column(type: "int", nullable: false),
                    Timestamp = table.Column(type: "datetime2", nullable: false),
                    Type = table.Column(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerUpdate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerUpdate_Customer_CustomerId",
                        columns: x => x.CustomerId,
                        referencedTable: "Customer",
                        referencedColumn: "Id");
                });
        }
        
        public override void Down(MigrationBuilder migration)
        {
            migration.DropSequence("DefaultSequence");
            migration.DropTable("Customer");
            migration.DropTable("CustomerUpdate");
        }
    }
}
