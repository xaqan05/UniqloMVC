﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniqloMVC.Migrations
{
    /// <inheritdoc />
    public partial class CreatedRegister : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Passwoord",
                table: "AspNetUsers",
                newName: "ProfileImageUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfileImageUrl",
                table: "AspNetUsers",
                newName: "Passwoord");
        }
    }
}
