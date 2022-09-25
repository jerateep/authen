using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace auth.models.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ip_from",
                table: "TBL_Hmac");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "TBL_Hmac",
                newName: "serviceName");

            migrationBuilder.AlterColumn<string>(
                name: "hmac",
                table: "TBL_Hmac",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "serviceName",
                table: "TBL_Hmac",
                newName: "username");

            migrationBuilder.AlterColumn<int>(
                name: "hmac",
                table: "TBL_Hmac",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ip_from",
                table: "TBL_Hmac",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
