using Microsoft.EntityFrameworkCore.Migrations;

namespace Models.Migrations
{
    public partial class addboundaryStake : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoundaryStakes",
                columns: table => new
                {
                    KJSJBSM = table.Column<string>(nullable: false),
                    JZBH = table.Column<string>(nullable: true),
                    JXDM = table.Column<string>(nullable: true),
                    ZZB = table.Column<double>(nullable: false),
                    HZB = table.Column<double>(nullable: false),
                    JD = table.Column<string>(nullable: true),
                    WD = table.Column<string>(nullable: true),
                    JZDJ = table.Column<string>(nullable: true),
                    JZLX = table.Column<string>(nullable: true),
                    JZCZ = table.Column<string>(nullable: true),
                    SZD1 = table.Column<string>(nullable: true),
                    GC = table.Column<float>(nullable: false),
                    YHFWWWZ = table.Column<string>(nullable: true),
                    YHFWWFWJ = table.Column<string>(nullable: true),
                    YHFWWJL = table.Column<string>(nullable: true),
                    EHFWWWZ = table.Column<string>(nullable: true),
                    EHFWWFWJ = table.Column<string>(nullable: true),
                    EHFWWJL = table.Column<string>(nullable: true),
                    SHFWWWZ = table.Column<string>(nullable: true),
                    SHFWWFWJ = table.Column<string>(nullable: true),
                    SHFWWJL = table.Column<string>(nullable: true),
                    WZMS = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoundaryStakes", x => x.KJSJBSM);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoundaryStakes");
        }
    }
}
