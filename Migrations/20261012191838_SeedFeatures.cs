using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Challenge_1_Build_APIS.Migrations
{
    /// <inheritdoc />
    public partial class SeedFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql("INSERT INTO Features (Name) VALUES ('Registered')");
            migrationBuilder.Sql("INSERT INTO Features (Name) VALUES ('AWD')");
            migrationBuilder.Sql("INSERT INTO Features (Name) VALUES ('Sunroof')");
            migrationBuilder.Sql("INSERT INTO Features (Name) VALUES ('Leather Seats')");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(

               "DELETE FROM Features WHERE Name IN ('Registered','AWD','Sunroof','Leather Seats')");

        }
    }
}
