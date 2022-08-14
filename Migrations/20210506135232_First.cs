using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace _4AutoMarket.Migrations
{
    public partial class First : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Atributes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    _Index = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atributes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Catalogs",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    _Index = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Position = table.Column<int>(nullable: false),
                    FatherCatalogId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalogs_Catalogs_FatherCatalogId",
                        column: x => x.FatherCatalogId,
                        principalTable: "Catalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GlobalSetings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EnableAllReviews = table.Column<bool>(nullable: false),
                    LiveChat_AutoResponse = table.Column<string>(nullable: true),
                    LiveChat_OperatorName = table.Column<string>(nullable: true),
                    ShippingOrderSum = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ShippingWhenMore = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ShippingWhenLess = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    IdramId = table.Column<string>(nullable: true),
                    IdramSecretKey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalSetings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HomePageSections",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    _Index = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    IsFixedData = table.Column<bool>(nullable: false),
                    Position = table.Column<int>(nullable: false),
                    SectionName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomePageSections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdramPaymentHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(nullable: false),
                    TransactionDate = table.Column<string>(nullable: true),
                    TransactionID = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<string>(nullable: true),
                    IdramID = table.Column<string>(nullable: true),
                    PayerIdramId = table.Column<string>(nullable: true),
                    OrderPayedSum = table.Column<string>(nullable: true),
                    CheckSum = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdramPaymentHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LiveChatSessions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(nullable: true),
                    OperatorName = table.Column<string>(nullable: true),
                    Added = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveChatSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailingHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    to = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    MessageCount = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailingHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromoCodes",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    SalePercent = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    isActive = table.Column<bool>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Expired = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "RequestCells",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Number = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    isNew = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestCells", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestEmails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    isNew = table.Column<bool>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestEmails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SerchKeys",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(nullable: true),
                    CatalogId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerchKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SharedMessage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    SendedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedMessage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscribeEmails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: true),
                    isNew = table.Column<bool>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscribeEmails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AtributeValues",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    _Index = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Value = table.Column<string>(nullable: true),
                    FK_AtributeId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtributeValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AtributeValues_Atributes_FK_AtributeId",
                        column: x => x.FK_AtributeId,
                        principalTable: "Atributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    _Index = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    LinkAtributeId = table.Column<string>(nullable: true),
                    CatalogId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Catalogs_CatalogId",
                        column: x => x.CatalogId,
                        principalTable: "Catalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Atributes_LinkAtributeId",
                        column: x => x.LinkAtributeId,
                        principalTable: "Atributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HomePageSectionDatas",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    _Index = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Img = table.Column<byte[]>(nullable: true),
                    Added = table.Column<DateTime>(nullable: false),
                    TextIsWhith = table.Column<bool>(nullable: false),
                    CatalogId = table.Column<string>(nullable: true),
                    FK_HomePageSectionId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomePageSectionDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HomePageSectionDatas_Catalogs_CatalogId",
                        column: x => x.CatalogId,
                        principalTable: "Catalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HomePageSectionDatas_HomePageSections_FK_HomePageSectionId",
                        column: x => x.FK_HomePageSectionId,
                        principalTable: "HomePageSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LiveChatMessage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Message = table.Column<string>(nullable: true),
                    IsOperator = table.Column<bool>(nullable: false),
                    IsNew = table.Column<bool>(nullable: false),
                    Sended = table.Column<DateTime>(nullable: false),
                    LiveChatSessionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveChatMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiveChatMessage_LiveChatSessions_LiveChatSessionId",
                        column: x => x.LiveChatSessionId,
                        principalTable: "LiveChatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductTypes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    _Index = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SerchKeys = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Sale = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsMine = table.Column<bool>(nullable: false),
                    LinkAtributeValueId = table.Column<string>(nullable: true),
                    FirstAtributeId = table.Column<string>(nullable: true),
                    ProductId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductTypes_Atributes_FirstAtributeId",
                        column: x => x.FirstAtributeId,
                        principalTable: "Atributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductTypes_AtributeValues_LinkAtributeValueId",
                        column: x => x.LinkAtributeValueId,
                        principalTable: "AtributeValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductTypes_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CultureDatas",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Culture = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    FK_TitleId = table.Column<string>(nullable: true),
                    FK_BrandId = table.Column<string>(nullable: true),
                    FK_DescriptionId = table.Column<string>(nullable: true),
                    FK_HP_TitleId = table.Column<string>(nullable: true),
                    FK_HP_DescriptionId = table.Column<string>(nullable: true),
                    AtributeId = table.Column<string>(nullable: true),
                    AtributeValueId = table.Column<string>(nullable: true),
                    CatalogId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CultureDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CultureDatas_Atributes_AtributeId",
                        column: x => x.AtributeId,
                        principalTable: "Atributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CultureDatas_AtributeValues_AtributeValueId",
                        column: x => x.AtributeValueId,
                        principalTable: "AtributeValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CultureDatas_Catalogs_CatalogId",
                        column: x => x.CatalogId,
                        principalTable: "Catalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CultureDatas_ProductTypes_FK_BrandId",
                        column: x => x.FK_BrandId,
                        principalTable: "ProductTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CultureDatas_ProductTypes_FK_DescriptionId",
                        column: x => x.FK_DescriptionId,
                        principalTable: "ProductTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CultureDatas_HomePageSectionDatas_FK_HP_DescriptionId",
                        column: x => x.FK_HP_DescriptionId,
                        principalTable: "HomePageSectionDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CultureDatas_HomePageSectionDatas_FK_HP_TitleId",
                        column: x => x.FK_HP_TitleId,
                        principalTable: "HomePageSectionDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CultureDatas_ProductTypes_FK_TitleId",
                        column: x => x.FK_TitleId,
                        principalTable: "ProductTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductAtributes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ProductQuantity = table.Column<int>(nullable: false),
                    ProductTypeId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAtributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAtributes_ProductTypes_ProductTypeId",
                        column: x => x.ProductTypeId,
                        principalTable: "ProductTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductIMGs",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    _Index = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IMG = table.Column<byte[]>(nullable: true),
                    ProductTypeId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductIMGs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductIMGs_ProductTypes_ProductTypeId",
                        column: x => x.ProductTypeId,
                        principalTable: "ProductTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductAtributeValues",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    _Index = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AtributeValueId = table.Column<string>(nullable: true),
                    ProductAtributesId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAtributeValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAtributeValues_AtributeValues_AtributeValueId",
                        column: x => x.AtributeValueId,
                        principalTable: "AtributeValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductAtributeValues_ProductAtributes_ProductAtributesId",
                        column: x => x.ProductAtributesId,
                        principalTable: "ProductAtributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    _Index = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    Registred = table.Column<DateTime>(nullable: false),
                    Document = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    PostCode = table.Column<string>(nullable: true),
                    SelectedAddressId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AddressBook",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FName = table.Column<string>(nullable: true),
                    LName = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    PostCode = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressBook", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AddressBook_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    IsNew = table.Column<bool>(nullable: false),
                    SendedDate = table.Column<DateTime>(nullable: false),
                    FK_UserSendId = table.Column<string>(nullable: true),
                    FK_AdminSendId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Message_AspNetUsers_FK_AdminSendId",
                        column: x => x.FK_AdminSendId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Message_AspNetUsers_FK_UserSendId",
                        column: x => x.FK_UserSendId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    OrderNumber = table.Column<long>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    PaymentMethod = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    ProductsSum = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ShipingSum = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PromoSale = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PromoCode = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Rating = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    FK_UserId = table.Column<string>(nullable: true),
                    FK_ProductId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Products_FK_ProductId",
                        column: x => x.FK_ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Reviews_AspNetUsers_FK_UserId",
                        column: x => x.FK_UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SerchHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(nullable: false),
                    Key = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerchHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SerchHistories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SharedMessageUser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsNew = table.Column<bool>(nullable: false),
                    MessageId = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedMessageUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SharedMessageUser_SharedMessage_MessageId",
                        column: x => x.MessageId,
                        principalTable: "SharedMessage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SharedMessageUser_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderProductInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductImgId = table.Column<string>(nullable: true),
                    ProductTitle = table.Column<string>(nullable: true),
                    Brand = table.Column<string>(nullable: true),
                    ProductDescription = table.Column<string>(nullable: true),
                    ProductSum = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    OrderId = table.Column<string>(nullable: true),
                    ProductId = table.Column<string>(nullable: true),
                    ProductTypeId = table.Column<string>(nullable: true),
                    ProductAtributesId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProductInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderProductInfo_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OrderProductInfo_ProductAtributes_ProductAtributesId",
                        column: x => x.ProductAtributesId,
                        principalTable: "ProductAtributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderProductInfo_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderProductInfo_ProductTypes_ProductTypeId",
                        column: x => x.ProductTypeId,
                        principalTable: "ProductTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderAtributeValue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Atribute = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    AtributeValueId = table.Column<string>(nullable: true),
                    ProductInfoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderAtributeValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderAtributeValue_AtributeValues_AtributeValueId",
                        column: x => x.AtributeValueId,
                        principalTable: "AtributeValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderAtributeValue_OrderProductInfo_ProductInfoId",
                        column: x => x.ProductInfoId,
                        principalTable: "OrderProductInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddressBook_UserId",
                table: "AddressBook",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SelectedAddressId",
                table: "AspNetUsers",
                column: "SelectedAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_AtributeValues_FK_AtributeId",
                table: "AtributeValues",
                column: "FK_AtributeId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalogs_FatherCatalogId",
                table: "Catalogs",
                column: "FatherCatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_CultureDatas_AtributeId",
                table: "CultureDatas",
                column: "AtributeId");

            migrationBuilder.CreateIndex(
                name: "IX_CultureDatas_AtributeValueId",
                table: "CultureDatas",
                column: "AtributeValueId");

            migrationBuilder.CreateIndex(
                name: "IX_CultureDatas_CatalogId",
                table: "CultureDatas",
                column: "CatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_CultureDatas_FK_BrandId",
                table: "CultureDatas",
                column: "FK_BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_CultureDatas_FK_DescriptionId",
                table: "CultureDatas",
                column: "FK_DescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_CultureDatas_FK_HP_DescriptionId",
                table: "CultureDatas",
                column: "FK_HP_DescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_CultureDatas_FK_HP_TitleId",
                table: "CultureDatas",
                column: "FK_HP_TitleId");

            migrationBuilder.CreateIndex(
                name: "IX_CultureDatas_FK_TitleId",
                table: "CultureDatas",
                column: "FK_TitleId");

            migrationBuilder.CreateIndex(
                name: "IX_HomePageSectionDatas_CatalogId",
                table: "HomePageSectionDatas",
                column: "CatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_HomePageSectionDatas_FK_HomePageSectionId",
                table: "HomePageSectionDatas",
                column: "FK_HomePageSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveChatMessage_LiveChatSessionId",
                table: "LiveChatMessage",
                column: "LiveChatSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_FK_AdminSendId",
                table: "Message",
                column: "FK_AdminSendId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_FK_UserSendId",
                table: "Message",
                column: "FK_UserSendId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAtributeValue_AtributeValueId",
                table: "OrderAtributeValue",
                column: "AtributeValueId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAtributeValue_ProductInfoId",
                table: "OrderAtributeValue",
                column: "ProductInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProductInfo_OrderId",
                table: "OrderProductInfo",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProductInfo_ProductAtributesId",
                table: "OrderProductInfo",
                column: "ProductAtributesId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProductInfo_ProductId",
                table: "OrderProductInfo",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProductInfo_ProductTypeId",
                table: "OrderProductInfo",
                column: "ProductTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAtributes_ProductTypeId",
                table: "ProductAtributes",
                column: "ProductTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAtributeValues_AtributeValueId",
                table: "ProductAtributeValues",
                column: "AtributeValueId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAtributeValues_ProductAtributesId",
                table: "ProductAtributeValues",
                column: "ProductAtributesId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductIMGs_ProductTypeId",
                table: "ProductIMGs",
                column: "ProductTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CatalogId",
                table: "Products",
                column: "CatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_LinkAtributeId",
                table: "Products",
                column: "LinkAtributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypes_FirstAtributeId",
                table: "ProductTypes",
                column: "FirstAtributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypes_LinkAtributeValueId",
                table: "ProductTypes",
                column: "LinkAtributeValueId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypes_ProductId",
                table: "ProductTypes",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_FK_ProductId",
                table: "Reviews",
                column: "FK_ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_FK_UserId",
                table: "Reviews",
                column: "FK_UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SerchHistories_UserId",
                table: "SerchHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedMessageUser_MessageId",
                table: "SharedMessageUser",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedMessageUser_UserId",
                table: "SharedMessageUser",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AddressBook_SelectedAddressId",
                table: "AspNetUsers",
                column: "SelectedAddressId",
                principalTable: "AddressBook",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddressBook_AspNetUsers_UserId",
                table: "AddressBook");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CultureDatas");

            migrationBuilder.DropTable(
                name: "GlobalSetings");

            migrationBuilder.DropTable(
                name: "IdramPaymentHistories");

            migrationBuilder.DropTable(
                name: "LiveChatMessage");

            migrationBuilder.DropTable(
                name: "MailingHistories");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "OrderAtributeValue");

            migrationBuilder.DropTable(
                name: "ProductAtributeValues");

            migrationBuilder.DropTable(
                name: "ProductIMGs");

            migrationBuilder.DropTable(
                name: "PromoCodes");

            migrationBuilder.DropTable(
                name: "RequestCells");

            migrationBuilder.DropTable(
                name: "RequestEmails");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "SerchHistories");

            migrationBuilder.DropTable(
                name: "SerchKeys");

            migrationBuilder.DropTable(
                name: "SharedMessageUser");

            migrationBuilder.DropTable(
                name: "SubscribeEmails");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "HomePageSectionDatas");

            migrationBuilder.DropTable(
                name: "LiveChatSessions");

            migrationBuilder.DropTable(
                name: "OrderProductInfo");

            migrationBuilder.DropTable(
                name: "SharedMessage");

            migrationBuilder.DropTable(
                name: "HomePageSections");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "ProductAtributes");

            migrationBuilder.DropTable(
                name: "ProductTypes");

            migrationBuilder.DropTable(
                name: "AtributeValues");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Catalogs");

            migrationBuilder.DropTable(
                name: "Atributes");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "AddressBook");
        }
    }
}
