using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Service.Community.Domain.Migrations
{
    /// <inheritdoc />
    public partial class CoreEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Service.Community");

            migrationBuilder.CreateTable(
                name: "Forums",
                schema: "Service.Community",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SubId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forums", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prefixes",
                schema: "Service.Community",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Foreground = table.Column<string>(type: "text", nullable: false),
                    Background = table.Column<string>(type: "text", nullable: false),
                    SubId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prefixes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                schema: "Service.Community",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ForumId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    SubId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Topics_Forums_ForumId",
                        column: x => x.ForumId,
                        principalSchema: "Service.Community",
                        principalTable: "Forums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                schema: "Service.Community",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ForumId = table.Column<Guid>(type: "uuid", nullable: false),
                    TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrefixId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Body = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    SubId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Forums_ForumId",
                        column: x => x.ForumId,
                        principalSchema: "Service.Community",
                        principalTable: "Forums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_Prefixes_PrefixId",
                        column: x => x.PrefixId,
                        principalSchema: "Service.Community",
                        principalTable: "Prefixes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Posts_Topics_TopicId",
                        column: x => x.TopicId,
                        principalSchema: "Service.Community",
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostVotes",
                schema: "Service.Community",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    SubId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostVotes_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "Service.Community",
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Replies",
                schema: "Service.Community",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Body = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false),
                    SubId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Replies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Replies_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "Service.Community",
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Replies_Replies_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "Service.Community",
                        principalTable: "Replies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReplyVotes",
                schema: "Service.Community",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReplyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    SubId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplyVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReplyVotes_Replies_ReplyId",
                        column: x => x.ReplyId,
                        principalSchema: "Service.Community",
                        principalTable: "Replies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Forums_SubId",
                schema: "Service.Community",
                table: "Forums",
                column: "SubId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ForumId",
                schema: "Service.Community",
                table: "Posts",
                column: "ForumId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_PrefixId",
                schema: "Service.Community",
                table: "Posts",
                column: "PrefixId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_SubId",
                schema: "Service.Community",
                table: "Posts",
                column: "SubId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_TopicId",
                schema: "Service.Community",
                table: "Posts",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_PostVotes_PostId",
                schema: "Service.Community",
                table: "PostVotes",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostVotes_SubId",
                schema: "Service.Community",
                table: "PostVotes",
                column: "SubId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prefixes_SubId",
                schema: "Service.Community",
                table: "Prefixes",
                column: "SubId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Replies_ParentId",
                schema: "Service.Community",
                table: "Replies",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Replies_PostId",
                schema: "Service.Community",
                table: "Replies",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Replies_SubId",
                schema: "Service.Community",
                table: "Replies",
                column: "SubId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReplyVotes_ReplyId",
                schema: "Service.Community",
                table: "ReplyVotes",
                column: "ReplyId");

            migrationBuilder.CreateIndex(
                name: "IX_ReplyVotes_SubId",
                schema: "Service.Community",
                table: "ReplyVotes",
                column: "SubId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Topics_ForumId",
                schema: "Service.Community",
                table: "Topics",
                column: "ForumId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_SubId",
                schema: "Service.Community",
                table: "Topics",
                column: "SubId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostVotes",
                schema: "Service.Community");

            migrationBuilder.DropTable(
                name: "ReplyVotes",
                schema: "Service.Community");

            migrationBuilder.DropTable(
                name: "Replies",
                schema: "Service.Community");

            migrationBuilder.DropTable(
                name: "Posts",
                schema: "Service.Community");

            migrationBuilder.DropTable(
                name: "Prefixes",
                schema: "Service.Community");

            migrationBuilder.DropTable(
                name: "Topics",
                schema: "Service.Community");

            migrationBuilder.DropTable(
                name: "Forums",
                schema: "Service.Community");
        }
    }
}
