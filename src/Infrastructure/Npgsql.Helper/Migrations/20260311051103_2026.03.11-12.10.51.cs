using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FoodSphere.Npgsql.Helper.Migrations
{
    /// <inheritdoc />
    public partial class _20260311121051 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConsumerUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumerUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coupon",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: false),
                    PercentageDiscount = table.Column<decimal>(type: "numeric(7,4)", nullable: true),
                    FixedDiscount = table.Column<int>(type: "integer", nullable: true),
                    MaxUsage = table.Column<int>(type: "integer", nullable: true),
                    StartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupon", x => new { x.RestaurantId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "InboxState",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsumerId = table.Column<Guid>(type: "uuid", nullable: false),
                    LockId = table.Column<Guid>(type: "uuid", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    Received = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReceiveCount = table.Column<int>(type: "integer", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Consumed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Delivered = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxState", x => x.Id);
                    table.UniqueConstraint("AK_InboxState_MessageId_ConsumerId", x => new { x.MessageId, x.ConsumerId });
                });

            migrationBuilder.CreateTable(
                name: "OutboxState",
                columns: table => new
                {
                    OutboxId = table.Column<Guid>(type: "uuid", nullable: false),
                    LockId = table.Column<Guid>(type: "uuid", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Delivered = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxState", x => x.OutboxId);
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
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
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
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
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
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
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
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
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
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
                name: "Restaurant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OwnerId = table.Column<string>(type: "text", nullable: false),
                    Contact_Name = table.Column<string>(type: "text", nullable: true),
                    Contact_Email = table.Column<string>(type: "text", nullable: true),
                    Contact_Phone = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StripeAccountId = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Restaurant_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessage",
                columns: table => new
                {
                    SequenceNumber = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EnqueueTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SentTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Headers = table.Column<string>(type: "text", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    InboxMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    InboxConsumerId = table.Column<Guid>(type: "uuid", nullable: true),
                    OutboxId = table.Column<Guid>(type: "uuid", nullable: true),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    MessageType = table.Column<string>(type: "text", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: true),
                    InitiatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestId = table.Column<Guid>(type: "uuid", nullable: true),
                    SourceAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DestinationAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ResponseAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    FaultAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessage", x => x.SequenceNumber);
                    table.ForeignKey(
                        name: "FK_OutboxMessage_InboxState_InboxMessageId_InboxConsumerId",
                        columns: x => new { x.InboxMessageId, x.InboxConsumerId },
                        principalTable: "InboxState",
                        principalColumns: new[] { "MessageId", "ConsumerId" });
                    table.ForeignKey(
                        name: "FK_OutboxMessage_OutboxState_OutboxId",
                        column: x => x.OutboxId,
                        principalTable: "OutboxState",
                        principalColumn: "OutboxId");
                });

            migrationBuilder.CreateTable(
                name: "Branch",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Contact_Name = table.Column<string>(type: "text", nullable: true),
                    Contact_Email = table.Column<string>(type: "text", nullable: true),
                    Contact_Phone = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    OpeningTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    ClosingTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branch", x => new { x.RestaurantId, x.Id });
                    table.ForeignKey(
                        name: "FK_Branch_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ingredient",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredient", x => new { x.RestaurantId, x.Id });
                    table.ForeignKey(
                        name: "FK_Ingredient_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Menu",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menu", x => new { x.RestaurantId, x.Id });
                    table.ForeignKey(
                        name: "FK_Menu_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantStaff",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    MasterId = table.Column<string>(type: "text", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantStaff", x => new { x.RestaurantId, x.MasterId });
                    table.ForeignKey(
                        name: "FK_RestaurantStaff_AspNetUsers_MasterId",
                        column: x => x.MasterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RestaurantStaff_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => new { x.RestaurantId, x.Id });
                    table.ForeignKey(
                        name: "FK_Role_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => new { x.RestaurantId, x.Id });
                    table.ForeignKey(
                        name: "FK_Tag_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BranchStaff",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<short>(type: "smallint", nullable: false),
                    MasterId = table.Column<string>(type: "text", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchStaff", x => new { x.RestaurantId, x.BranchId, x.MasterId });
                    table.ForeignKey(
                        name: "FK_BranchStaff_AspNetUsers_MasterId",
                        column: x => x.MasterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BranchStaff_Branch_RestaurantId_BranchId",
                        columns: x => new { x.RestaurantId, x.BranchId },
                        principalTable: "Branch",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Queuing",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<short>(type: "smallint", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ConsumerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Pax = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Queuing", x => new { x.RestaurantId, x.BranchId, x.Id });
                    table.ForeignKey(
                        name: "FK_Queuing_Branch_RestaurantId_BranchId",
                        columns: x => new { x.RestaurantId, x.BranchId },
                        principalTable: "Branch",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Queuing_ConsumerUser_ConsumerId",
                        column: x => x.ConsumerId,
                        principalTable: "ConsumerUser",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Table",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<short>(type: "smallint", nullable: false),
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Table", x => new { x.RestaurantId, x.BranchId, x.Id });
                    table.ForeignKey(
                        name: "FK_Table_Branch_RestaurantId_BranchId",
                        columns: x => new { x.RestaurantId, x.BranchId },
                        principalTable: "Branch",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkerUser",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<short>(type: "smallint", nullable: false),
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Position = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerUser", x => new { x.RestaurantId, x.BranchId, x.Id });
                    table.ForeignKey(
                        name: "FK_WorkerUser_Branch_RestaurantId_BranchId",
                        columns: x => new { x.RestaurantId, x.BranchId },
                        principalTable: "Branch",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Stock",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<short>(type: "smallint", nullable: false),
                    IngredientId = table.Column<short>(type: "smallint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(10,4)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stock", x => new { x.RestaurantId, x.BranchId, x.IngredientId });
                    table.ForeignKey(
                        name: "FK_Stock_Branch_RestaurantId_BranchId",
                        columns: x => new { x.RestaurantId, x.BranchId },
                        principalTable: "Branch",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stock_Ingredient_RestaurantId_IngredientId",
                        columns: x => new { x.RestaurantId, x.IngredientId },
                        principalTable: "Ingredient",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuComponent",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentMenuId = table.Column<short>(type: "smallint", nullable: false),
                    ChildMenuId = table.Column<short>(type: "smallint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Quantity = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuComponent", x => new { x.RestaurantId, x.ParentMenuId, x.ChildMenuId });
                    table.ForeignKey(
                        name: "FK_MenuComponent_Menu_RestaurantId_ChildMenuId",
                        columns: x => new { x.RestaurantId, x.ChildMenuId },
                        principalTable: "Menu",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuComponent_Menu_RestaurantId_ParentMenuId",
                        columns: x => new { x.RestaurantId, x.ParentMenuId },
                        principalTable: "Menu",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuComponent_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuIngredient",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    MenuId = table.Column<short>(type: "smallint", nullable: false),
                    IngredientId = table.Column<short>(type: "smallint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(10,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuIngredient", x => new { x.RestaurantId, x.MenuId, x.IngredientId });
                    table.ForeignKey(
                        name: "FK_MenuIngredient_Ingredient_RestaurantId_IngredientId",
                        columns: x => new { x.RestaurantId, x.IngredientId },
                        principalTable: "Ingredient",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuIngredient_Menu_RestaurantId_MenuId",
                        columns: x => new { x.RestaurantId, x.MenuId },
                        principalTable: "Menu",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantStaffRole",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    MasterId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<short>(type: "smallint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantStaffRole", x => new { x.RestaurantId, x.MasterId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_RestaurantStaffRole_RestaurantStaff_RestaurantId_MasterId",
                        columns: x => new { x.RestaurantId, x.MasterId },
                        principalTable: "RestaurantStaff",
                        principalColumns: new[] { "RestaurantId", "MasterId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RestaurantStaffRole_Role_RestaurantId_RoleId",
                        columns: x => new { x.RestaurantId, x.RoleId },
                        principalTable: "Role",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePermission",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<short>(type: "smallint", nullable: false),
                    PermissionId = table.Column<int>(type: "integer", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => new { x.RestaurantId, x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermission_Permission_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermission_Role_RestaurantId_RoleId",
                        columns: x => new { x.RestaurantId, x.RoleId },
                        principalTable: "Role",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IngredientTag",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    IngredientId = table.Column<short>(type: "smallint", nullable: false),
                    TagId = table.Column<short>(type: "smallint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientTag", x => new { x.RestaurantId, x.IngredientId, x.TagId });
                    table.ForeignKey(
                        name: "FK_IngredientTag_Ingredient_RestaurantId_IngredientId",
                        columns: x => new { x.RestaurantId, x.IngredientId },
                        principalTable: "Ingredient",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngredientTag_Tag_RestaurantId_TagId",
                        columns: x => new { x.RestaurantId, x.TagId },
                        principalTable: "Tag",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MenuTag",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    MenuId = table.Column<short>(type: "smallint", nullable: false),
                    TagId = table.Column<short>(type: "smallint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuTag", x => new { x.RestaurantId, x.MenuId, x.TagId });
                    table.ForeignKey(
                        name: "FK_MenuTag_Menu_RestaurantId_MenuId",
                        columns: x => new { x.RestaurantId, x.MenuId },
                        principalTable: "Menu",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuTag_Tag_RestaurantId_TagId",
                        columns: x => new { x.RestaurantId, x.TagId },
                        principalTable: "Tag",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BranchStaffRole",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<short>(type: "smallint", nullable: false),
                    MasterId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<short>(type: "smallint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchStaffRole", x => new { x.RestaurantId, x.BranchId, x.MasterId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_BranchStaffRole_BranchStaff_RestaurantId_BranchId_MasterId",
                        columns: x => new { x.RestaurantId, x.BranchId, x.MasterId },
                        principalTable: "BranchStaff",
                        principalColumns: new[] { "RestaurantId", "BranchId", "MasterId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BranchStaffRole_Role_RestaurantId_RoleId",
                        columns: x => new { x.RestaurantId, x.RoleId },
                        principalTable: "Role",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bill",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<short>(type: "smallint", nullable: false),
                    TableId = table.Column<short>(type: "smallint", nullable: false),
                    IssuerId = table.Column<short>(type: "smallint", nullable: true),
                    IssuerRestaurantId = table.Column<Guid>(type: "uuid", nullable: true),
                    IssuerBranchId = table.Column<short>(type: "smallint", nullable: true),
                    ConsumerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Pax = table.Column<short>(type: "smallint", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bill_ConsumerUser_ConsumerId",
                        column: x => x.ConsumerId,
                        principalTable: "ConsumerUser",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Bill_Table_RestaurantId_BranchId_TableId",
                        columns: x => new { x.RestaurantId, x.BranchId, x.TableId },
                        principalTable: "Table",
                        principalColumns: new[] { "RestaurantId", "BranchId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bill_WorkerUser_IssuerRestaurantId_IssuerBranchId_IssuerId",
                        columns: x => new { x.IssuerRestaurantId, x.IssuerBranchId, x.IssuerId },
                        principalTable: "WorkerUser",
                        principalColumns: new[] { "RestaurantId", "BranchId", "Id" });
                    table.ForeignKey(
                        name: "FK_Bill_WorkerUser_RestaurantId_BranchId_IssuerId",
                        columns: x => new { x.RestaurantId, x.BranchId, x.IssuerId },
                        principalTable: "WorkerUser",
                        principalColumns: new[] { "RestaurantId", "BranchId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkerPortal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<short>(type: "smallint", nullable: false),
                    WorkerId = table.Column<short>(type: "smallint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsageCount = table.Column<short>(type: "smallint", nullable: false),
                    ValidDuration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    MaxUsage = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerPortal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkerPortal_WorkerUser_RestaurantId_BranchId_WorkerId",
                        columns: x => new { x.RestaurantId, x.BranchId, x.WorkerId },
                        principalTable: "WorkerUser",
                        principalColumns: new[] { "RestaurantId", "BranchId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkerRole",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<short>(type: "smallint", nullable: false),
                    WorkerId = table.Column<short>(type: "smallint", nullable: false),
                    RoleId = table.Column<short>(type: "smallint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerRole", x => new { x.RestaurantId, x.BranchId, x.WorkerId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_WorkerRole_Role_RestaurantId_RoleId",
                        columns: x => new { x.RestaurantId, x.RoleId },
                        principalTable: "Role",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkerRole_WorkerUser_RestaurantId_BranchId_WorkerId",
                        columns: x => new { x.RestaurantId, x.BranchId, x.WorkerId },
                        principalTable: "WorkerUser",
                        principalColumns: new[] { "RestaurantId", "BranchId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BillMember",
                columns: table => new
                {
                    BillId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ConsumerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillMember", x => new { x.BillId, x.Id });
                    table.ForeignKey(
                        name: "FK_BillMember_Bill_BillId",
                        column: x => x.BillId,
                        principalTable: "Bill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BillMember_ConsumerUser_ConsumerId",
                        column: x => x.ConsumerId,
                        principalTable: "ConsumerUser",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderingPortal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BillId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsageCount = table.Column<short>(type: "smallint", nullable: false),
                    ValidDuration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    MaxUsage = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderingPortal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderingPortal_Bill_BillId",
                        column: x => x.BillId,
                        principalTable: "Bill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    BillId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: true),
                    BranchId = table.Column<short>(type: "smallint", nullable: true),
                    IssuerId = table.Column<short>(type: "smallint", nullable: true),
                    IssuerRestaurantId = table.Column<Guid>(type: "uuid", nullable: true),
                    IssuerBranchId = table.Column<short>(type: "smallint", nullable: true),
                    BillMemberId = table.Column<short>(type: "smallint", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => new { x.BillId, x.Id });
                    table.ForeignKey(
                        name: "FK_Order_BillMember_BillId_BillMemberId",
                        columns: x => new { x.BillId, x.BillMemberId },
                        principalTable: "BillMember",
                        principalColumns: new[] { "BillId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_Bill_BillId",
                        column: x => x.BillId,
                        principalTable: "Bill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Order_WorkerUser_IssuerRestaurantId_IssuerBranchId_IssuerId",
                        columns: x => new { x.IssuerRestaurantId, x.IssuerBranchId, x.IssuerId },
                        principalTable: "WorkerUser",
                        principalColumns: new[] { "RestaurantId", "BranchId", "Id" });
                    table.ForeignKey(
                        name: "FK_Order_WorkerUser_RestaurantId_BranchId_IssuerId",
                        columns: x => new { x.RestaurantId, x.BranchId, x.IssuerId },
                        principalTable: "WorkerUser",
                        principalColumns: new[] { "RestaurantId", "BranchId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    BillId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<short>(type: "smallint", nullable: false),
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    MenuId = table.Column<short>(type: "smallint", nullable: false),
                    NameSnapshot = table.Column<string>(type: "text", nullable: false),
                    PriceSnapshot = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<short>(type: "smallint", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => new { x.BillId, x.OrderId, x.Id });
                    table.ForeignKey(
                        name: "FK_OrderItem_Menu_RestaurantId_MenuId",
                        columns: x => new { x.RestaurantId, x.MenuId },
                        principalTable: "Menu",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItem_Order_BillId_OrderId",
                        columns: x => new { x.BillId, x.OrderId },
                        principalTable: "Order",
                        principalColumns: new[] { "BillId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "DeleteTime", "Description", "Name", "UpdateTime" },
                values: new object[,]
                {
                    { 1000, null, null, "restaurant.setting.read", null },
                    { 1010, null, null, "restaurant.setting.update", null },
                    { 2000, null, null, "ingredient.create", null },
                    { 2010, null, null, "ingredient.read", null },
                    { 2020, null, null, "ingredient.update", null },
                    { 3000, null, null, "menu.create", null },
                    { 3010, null, null, "menu.update", null },
                    { 4000, null, null, "branch.setting.read", null },
                    { 4010, null, null, "branch.setting.update", null },
                    { 5000, null, null, "stock.read", null },
                    { 5010, null, null, "stock.update", null },
                    { 6000, null, null, "table.create", null },
                    { 6010, null, null, "table.update", null },
                    { 7000, null, null, "order.create", null },
                    { 7010, null, null, "order.get", null },
                    { 7020, null, null, "order.list", null },
                    { 7030, null, null, "order.update", null },
                    { 8000, null, null, "dashboard.read", null },
                    { 9000, null, null, "role.create", null },
                    { 9010, null, null, "role.read", null },
                    { 9020, null, null, "role.update", null },
                    { 9030, null, null, "role.delete", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

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
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bill_ConsumerId",
                table: "Bill",
                column: "ConsumerId");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_IssuerRestaurantId_IssuerBranchId_IssuerId",
                table: "Bill",
                columns: new[] { "IssuerRestaurantId", "IssuerBranchId", "IssuerId" });

            migrationBuilder.CreateIndex(
                name: "IX_Bill_RestaurantId_BranchId_IssuerId",
                table: "Bill",
                columns: new[] { "RestaurantId", "BranchId", "IssuerId" });

            migrationBuilder.CreateIndex(
                name: "IX_Bill_RestaurantId_BranchId_TableId",
                table: "Bill",
                columns: new[] { "RestaurantId", "BranchId", "TableId" });

            migrationBuilder.CreateIndex(
                name: "IX_BillMember_BillId_ConsumerId",
                table: "BillMember",
                columns: new[] { "BillId", "ConsumerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BillMember_ConsumerId",
                table: "BillMember",
                column: "ConsumerId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchStaff_MasterId",
                table: "BranchStaff",
                column: "MasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchStaffRole_RestaurantId_RoleId",
                table: "BranchStaffRole",
                columns: new[] { "RestaurantId", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_ConsumerUser_Email",
                table: "ConsumerUser",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConsumerUser_UserName",
                table: "ConsumerUser",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_Code",
                table: "Coupon",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InboxState_Delivered",
                table: "InboxState",
                column: "Delivered");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTag_RestaurantId_TagId",
                table: "IngredientTag",
                columns: new[] { "RestaurantId", "TagId" });

            migrationBuilder.CreateIndex(
                name: "IX_MenuComponent_RestaurantId_ChildMenuId",
                table: "MenuComponent",
                columns: new[] { "RestaurantId", "ChildMenuId" });

            migrationBuilder.CreateIndex(
                name: "IX_MenuIngredient_RestaurantId_IngredientId",
                table: "MenuIngredient",
                columns: new[] { "RestaurantId", "IngredientId" });

            migrationBuilder.CreateIndex(
                name: "IX_MenuTag_RestaurantId_TagId",
                table: "MenuTag",
                columns: new[] { "RestaurantId", "TagId" });

            migrationBuilder.CreateIndex(
                name: "IX_Order_BillId_BillMemberId",
                table: "Order",
                columns: new[] { "BillId", "BillMemberId" });

            migrationBuilder.CreateIndex(
                name: "IX_Order_IssuerRestaurantId_IssuerBranchId_IssuerId",
                table: "Order",
                columns: new[] { "IssuerRestaurantId", "IssuerBranchId", "IssuerId" });

            migrationBuilder.CreateIndex(
                name: "IX_Order_RestaurantId_BranchId_IssuerId",
                table: "Order",
                columns: new[] { "RestaurantId", "BranchId", "IssuerId" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderingPortal_BillId",
                table: "OrderingPortal",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_RestaurantId_MenuId",
                table: "OrderItem",
                columns: new[] { "RestaurantId", "MenuId" });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_EnqueueTime",
                table: "OutboxMessage",
                column: "EnqueueTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_ExpirationTime",
                table: "OutboxMessage",
                column: "ExpirationTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_InboxMessageId_InboxConsumerId_SequenceNumber",
                table: "OutboxMessage",
                columns: new[] { "InboxMessageId", "InboxConsumerId", "SequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_OutboxId_SequenceNumber",
                table: "OutboxMessage",
                columns: new[] { "OutboxId", "SequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxState_Created",
                table: "OutboxState",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_Name",
                table: "Permission",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Queuing_ConsumerId",
                table: "Queuing",
                column: "ConsumerId");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurant_OwnerId",
                table: "Restaurant",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantStaff_MasterId",
                table: "RestaurantStaff",
                column: "MasterId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantStaffRole_RestaurantId_RoleId",
                table: "RestaurantStaffRole",
                columns: new[] { "RestaurantId", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_Role_RestaurantId_Name",
                table: "Role",
                columns: new[] { "RestaurantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_PermissionId",
                table: "RolePermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Stock_RestaurantId_IngredientId",
                table: "Stock",
                columns: new[] { "RestaurantId", "IngredientId" });

            migrationBuilder.CreateIndex(
                name: "IX_Tag_RestaurantId_Name",
                table: "Tag",
                columns: new[] { "RestaurantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkerPortal_RestaurantId_BranchId_WorkerId",
                table: "WorkerPortal",
                columns: new[] { "RestaurantId", "BranchId", "WorkerId" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkerRole_RestaurantId_RoleId",
                table: "WorkerRole",
                columns: new[] { "RestaurantId", "RoleId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "BranchStaffRole");

            migrationBuilder.DropTable(
                name: "Coupon");

            migrationBuilder.DropTable(
                name: "IngredientTag");

            migrationBuilder.DropTable(
                name: "MenuComponent");

            migrationBuilder.DropTable(
                name: "MenuIngredient");

            migrationBuilder.DropTable(
                name: "MenuTag");

            migrationBuilder.DropTable(
                name: "OrderingPortal");

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "OutboxMessage");

            migrationBuilder.DropTable(
                name: "Queuing");

            migrationBuilder.DropTable(
                name: "RestaurantStaffRole");

            migrationBuilder.DropTable(
                name: "RolePermission");

            migrationBuilder.DropTable(
                name: "Stock");

            migrationBuilder.DropTable(
                name: "WorkerPortal");

            migrationBuilder.DropTable(
                name: "WorkerRole");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "BranchStaff");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "Menu");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "InboxState");

            migrationBuilder.DropTable(
                name: "OutboxState");

            migrationBuilder.DropTable(
                name: "RestaurantStaff");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "Ingredient");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "BillMember");

            migrationBuilder.DropTable(
                name: "Bill");

            migrationBuilder.DropTable(
                name: "ConsumerUser");

            migrationBuilder.DropTable(
                name: "Table");

            migrationBuilder.DropTable(
                name: "WorkerUser");

            migrationBuilder.DropTable(
                name: "Branch");

            migrationBuilder.DropTable(
                name: "Restaurant");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
