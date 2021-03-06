// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ModelsBuildingEconomy.buildingCompany;

namespace ModelsBuildingEconomy.Migrations
{
    [DbContext(typeof(xjpCompanyContext))]
    [Migration("20201031084920_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ModelsBuildingEconomy.buildingCompany.BuildingFloor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BuildingName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Community")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CompanyBuildingId")
                        .HasColumnType("int");

                    b.Property<string>("FloorNum")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Height")
                        .HasColumnType("float");

                    b.Property<double>("Lat")
                        .HasColumnType("float");

                    b.Property<double>("Long")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("CompanyBuildingId");

                    b.ToTable("BuildingFloor");
                });

            modelBuilder.Entity("ModelsBuildingEconomy.buildingCompany.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ActualOfficeAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BusinessDirection")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CompanyBuildingId")
                        .HasColumnType("int");

                    b.Property<int?>("CompanyEconomyId")
                        .HasColumnType("int");

                    b.Property<string>("CompanyName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CompanyOtherInfoId")
                        .HasColumnType("int");

                    b.Property<int?>("CompanyTaxInfoId")
                        .HasColumnType("int");

                    b.Property<string>("Contacts")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EnterpriseBackground")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EnterpriseType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IsIndependentLegalEntity")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LegalRepresentative")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegisteredAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegisteredCapital")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegistrationPlace")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TaxStatisticsArea")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UnifiedSocialCreditCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("note")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CompanyBuildingId");

                    b.HasIndex("CompanyEconomyId");

                    b.HasIndex("CompanyOtherInfoId");

                    b.HasIndex("CompanyTaxInfoId");

                    b.ToTable("Company");
                });

            modelBuilder.Entity("ModelsBuildingEconomy.buildingCompany.CompanyBuilding", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BuildingName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StreetName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("CompanyBuilding");
                });

            modelBuilder.Entity("ModelsBuildingEconomy.buildingCompany.CompanyEconomy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CompanyName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CorporateTax")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UnifiedSocialCreditCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("duration")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("CompanyEconomy");
                });

            modelBuilder.Entity("ModelsBuildingEconomy.buildingCompany.CompanyOtherInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Area")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CompanyName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Floor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MoveAwayTime")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SettlingTime")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UnifiedSocialCreditCode")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("CompanyOtherInfo");
                });

            modelBuilder.Entity("ModelsBuildingEconomy.buildingCompany.CompanyTaxInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("AdditionalTaxOfEducation")
                        .HasColumnType("float");

                    b.Property<double>("BusinessTax")
                        .HasColumnType("float");

                    b.Property<double>("CorporateIncomeTax")
                        .HasColumnType("float");

                    b.Property<double>("DeedTax")
                        .HasColumnType("float");

                    b.Property<double>("DelayedTaxPayment")
                        .HasColumnType("float");

                    b.Property<double>("IndividualIncomeTax")
                        .HasColumnType("float");

                    b.Property<double>("LandUseTax")
                        .HasColumnType("float");

                    b.Property<double>("LandValueIncrementTax")
                        .HasColumnType("float");

                    b.Property<double>("RealEstateTax")
                        .HasColumnType("float");

                    b.Property<string>("RegisteredAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("StampDuty")
                        .HasColumnType("float");

                    b.Property<string>("TaxPayer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TaxYear")
                        .HasColumnType("int");

                    b.Property<double>("TotalTax")
                        .HasColumnType("float");

                    b.Property<string>("UnifiedSocialCreditCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("UrbanConstructionTax")
                        .HasColumnType("float");

                    b.Property<double>("ValueAddedTax")
                        .HasColumnType("float");

                    b.Property<double>("VehicleAndVesselTax")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("CompanyTaxInfo");
                });

            modelBuilder.Entity("ModelsBuildingEconomy.buildingCompany.BuildingFloor", b =>
                {
                    b.HasOne("ModelsBuildingEconomy.buildingCompany.CompanyBuilding", "CompanyBuilding")
                        .WithMany("Floor")
                        .HasForeignKey("CompanyBuildingId");
                });

            modelBuilder.Entity("ModelsBuildingEconomy.buildingCompany.Company", b =>
                {
                    b.HasOne("ModelsBuildingEconomy.buildingCompany.CompanyBuilding", "CompanyBuilding")
                        .WithMany("Company")
                        .HasForeignKey("CompanyBuildingId");

                    b.HasOne("ModelsBuildingEconomy.buildingCompany.CompanyEconomy", "CompanyEconomy")
                        .WithMany()
                        .HasForeignKey("CompanyEconomyId");

                    b.HasOne("ModelsBuildingEconomy.buildingCompany.CompanyOtherInfo", "CompanyOtherInfo")
                        .WithMany()
                        .HasForeignKey("CompanyOtherInfoId");

                    b.HasOne("ModelsBuildingEconomy.buildingCompany.CompanyTaxInfo", "CompanyTaxInfo")
                        .WithMany()
                        .HasForeignKey("CompanyTaxInfoId");
                });
#pragma warning restore 612, 618
        }
    }
}
