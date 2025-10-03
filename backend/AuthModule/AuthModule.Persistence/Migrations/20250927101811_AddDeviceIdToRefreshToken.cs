﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceIdToRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "device_id",
                table: "refresh_tokens",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "device_id",
                table: "refresh_tokens");
        }
    }
}
