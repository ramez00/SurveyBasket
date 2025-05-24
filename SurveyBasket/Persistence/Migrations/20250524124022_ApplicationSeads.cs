using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SurveyBasket.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationSeads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDelted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "905DF2BE-9722-402B-A141-2AE3F25A9842", "8B61025C-5CC6-4C69-963A-6C552DD586FB", false, false, "Admin", "ADMIN" },
                    { "A11D1435-FCBE-4773-BD26-521F8E054434", "F7815EED-96D0-46A2-AE5A-AB5E600C809E", true, false, "Member", "MEMBER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "3A638288-4AE4-4E82-9106-701030C9B7A1", 0, "cd822e8a-7a26-4c7b-aeb5-8a1c864ba7c7", "Survy-Basket@les-dev.net", true, "LES-Dev", "Admin", false, null, "SURVY-BASKET@LES-DEV.NET", "SURVY-BASKET@LES-DEV.NET", "AQAAAAIAAYagAAAAECWCu72XsddF/wHIDDax7jn9g9Oe7MffqcBwNTRyKPiw1Oe0GehcNQDPsHUow6hx0w==", null, false, "444e2399-160b-4364-b213-d54075866899", false, "Survy-Basket@les-dev.net" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "permission", "Polls:Read", "905DF2BE-9722-402B-A141-2AE3F25A9842" },
                    { 2, "permission", "Polls:Add", "905DF2BE-9722-402B-A141-2AE3F25A9842" },
                    { 3, "permission", "Polls:Edit", "905DF2BE-9722-402B-A141-2AE3F25A9842" },
                    { 4, "permission", "Polls:Delete", "905DF2BE-9722-402B-A141-2AE3F25A9842" },
                    { 5, "permission", "Questions:Read", "905DF2BE-9722-402B-A141-2AE3F25A9842" },
                    { 6, "permission", "Questions:Add", "905DF2BE-9722-402B-A141-2AE3F25A9842" },
                    { 7, "permission", "Questions:Edit", "905DF2BE-9722-402B-A141-2AE3F25A9842" },
                    { 8, "permission", "Users:Read", "905DF2BE-9722-402B-A141-2AE3F25A9842" },
                    { 9, "permission", "Users:Add", "905DF2BE-9722-402B-A141-2AE3F25A9842" },
                    { 10, "permission", "Users:Edit", "905DF2BE-9722-402B-A141-2AE3F25A9842" },
                    { 11, "permission", "Roles:Read", "905DF2BE-9722-402B-A141-2AE3F25A9842" },
                    { 12, "permission", "Roles:Add", "905DF2BE-9722-402B-A141-2AE3F25A9842" },
                    { 13, "permission", "Roles:Edit", "905DF2BE-9722-402B-A141-2AE3F25A9842" },
                    { 14, "permission", "Results:Read", "905DF2BE-9722-402B-A141-2AE3F25A9842" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "905DF2BE-9722-402B-A141-2AE3F25A9842", "3A638288-4AE4-4E82-9106-701030C9B7A1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "A11D1435-FCBE-4773-BD26-521F8E054434");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "905DF2BE-9722-402B-A141-2AE3F25A9842", "3A638288-4AE4-4E82-9106-701030C9B7A1" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "905DF2BE-9722-402B-A141-2AE3F25A9842");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3A638288-4AE4-4E82-9106-701030C9B7A1");
        }
    }
}
