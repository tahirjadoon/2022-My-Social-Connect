﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSC.Api.Core.DB.Migrations
{
    public partial class MessageEntityAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SenderId = table.Column<int>(type: "INTEGER", nullable: false),
                    SenderUsername = table.Column<string>(type: "TEXT", nullable: true),
                    SenderDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReceipientId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReceipientUsername = table.Column<string>(type: "TEXT", nullable: true),
                    ReceipientDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MessageContent = table.Column<string>(type: "TEXT", nullable: true),
                    DateMessageRead = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateMessageSent = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Users_ReceipientId",
                        column: x => x.ReceipientId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceipientId",
                table: "Messages",
                column: "ReceipientId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");
        }
    }
}
