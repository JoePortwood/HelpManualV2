using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HelpManual.Migrations
{
    public partial class DeleteFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormObject_ObjectType_ObjectTypeId",
                table: "FormObject");

            migrationBuilder.DropForeignKey(
                name: "FK_ObjectType_ControlType_ControlTypeId",
                table: "ObjectType");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ObjectType",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FormObject",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ControlType",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_FormObject_ObjectType_ObjectTypeId",
                table: "FormObject",
                column: "ObjectTypeId",
                principalTable: "ObjectType",
                principalColumn: "ObjectTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectType_ControlType_ControlTypeId",
                table: "ObjectType",
                column: "ControlTypeId",
                principalTable: "ControlType",
                principalColumn: "ControlTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormObject_ObjectType_ObjectTypeId",
                table: "FormObject");

            migrationBuilder.DropForeignKey(
                name: "FK_ObjectType_ControlType_ControlTypeId",
                table: "ObjectType");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ObjectType");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FormObject");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ControlType");

            migrationBuilder.AddForeignKey(
                name: "FK_FormObject_ObjectType_ObjectTypeId",
                table: "FormObject",
                column: "ObjectTypeId",
                principalTable: "ObjectType",
                principalColumn: "ObjectTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectType_ControlType_ControlTypeId",
                table: "ObjectType",
                column: "ControlTypeId",
                principalTable: "ControlType",
                principalColumn: "ControlTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
