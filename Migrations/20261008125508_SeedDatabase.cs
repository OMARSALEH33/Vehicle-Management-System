using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Challenge_1_Build_APIS.Migrations
{
    /// <inheritdoc />
    public partial class SeedDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Makes (Name) VALUES ('Make1')");
            migrationBuilder.Sql("INSERT INTO Makes (Name) VALUES ('Make2')");  
            migrationBuilder.Sql("INSERT INTO Makes (Name) VALUES ('Make3')");

            migrationBuilder.Sql("INSERT INTO Models (Name, MakeId) SELECT 'Model1-ModelA', Id FROM Makes WHERE Name = 'Make1'");
            migrationBuilder.Sql("INSERT INTO Models (Name, MakeId) SELECT 'Model1-ModelB', Id FROM Makes WHERE Name = 'Make1'");
            migrationBuilder.Sql("INSERT INTO Models (Name, MakeId) SELECT 'Model1-ModelC', Id FROM Makes WHERE Name = 'Make1'");

            migrationBuilder.Sql("INSERT INTO Models (Name, MakeId) SELECT 'Model2-ModelA', Id FROM Makes WHERE Name = 'Make2'");
            migrationBuilder.Sql("INSERT INTO Models (Name, MakeId) SELECT 'Model2-ModelB', Id FROM Makes WHERE Name = 'Make2'");
            migrationBuilder.Sql("INSERT INTO Models (Name, MakeId) SELECT 'Model2-ModelC', Id FROM Makes WHERE Name = 'Make2'");

            migrationBuilder.Sql("INSERT INTO Models (Name, MakeId) SELECT 'Model3-ModelA', Id FROM Makes WHERE Name = 'Make3'");
            migrationBuilder.Sql("INSERT INTO Models (Name, MakeId) SELECT 'Model3-ModelB', Id FROM Makes WHERE Name = 'Make3'");
            migrationBuilder.Sql("INSERT INTO Models (Name, MakeId) SELECT 'Model3-ModelC', Id FROM Makes WHERE Name = 'Make3'");
        }
        


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(
                 "DELETE FROM Models WHERE MakeId IN (SELECT Id FROM Makes WHERE Name IN ('Make1','Make2','Make3'))");
            migrationBuilder.Sql(
                "DELETE FROM Makes WHERE Name IN ('Make1','Make2','Make3')");
        }
    }
}
