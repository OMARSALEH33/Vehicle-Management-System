using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Challenge_1_Build_APIS.Migrations
{
    /// <inheritdoc />
    public partial class AddCodeToMake : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Makes",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE Makes SET Code = 'M' + CAST(Id AS nvarchar(10))");


            migrationBuilder.CreateIndex(
                name: "IX_Makes_Code",
                table: "Makes",
                column: "Code",
                unique: true);
          
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Makes_Code",
                table: "Makes");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Makes");
        }
    }
}
