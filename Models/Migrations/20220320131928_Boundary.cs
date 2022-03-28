using Microsoft.EntityFrameworkCore.Migrations;

namespace Models.Migrations
{
    public partial class Boundary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Boundarys",
                columns: table => new
                {
                    KJSJBSM = table.Column<string>(nullable: false),
                    JXDM = table.Column<string>(nullable: true),
                    JXDJ = table.Column<string>(nullable: true),
                    JXMC = table.Column<string>(nullable: true),
                    JXCD = table.Column<string>(nullable: true),
                    JXQD = table.Column<string>(nullable: true),
                    JXZD = table.Column<string>(nullable: true),
                    JZKS = table.Column<string>(nullable: true),
                    DLZKS = table.Column<string>(nullable: true),
                    SLZKS = table.Column<string>(nullable: true),
                    SANLZKS = table.Column<string>(nullable: true),
                    QTXX = table.Column<string>(nullable: true),
                    是否争议 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boundarys", x => x.KJSJBSM);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Boundarys");
        }
    }
}
