using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_ApplicationRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_voteAnswers_Answers_answerId",
                table: "voteAnswers");

            migrationBuilder.RenameColumn(
                name: "answerId",
                table: "voteAnswers",
                newName: "AnswerId");

            migrationBuilder.RenameIndex(
                name: "IX_voteAnswers_answerId",
                table: "voteAnswers",
                newName: "IX_voteAnswers_AnswerId");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "AspNetRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelted",
                table: "AspNetRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_voteAnswers_Answers_AnswerId",
                table: "voteAnswers",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_voteAnswers_Answers_AnswerId",
                table: "voteAnswers");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "IsDelted",
                table: "AspNetRoles");

            migrationBuilder.RenameColumn(
                name: "AnswerId",
                table: "voteAnswers",
                newName: "answerId");

            migrationBuilder.RenameIndex(
                name: "IX_voteAnswers_AnswerId",
                table: "voteAnswers",
                newName: "IX_voteAnswers_answerId");

            migrationBuilder.AddForeignKey(
                name: "FK_voteAnswers_Answers_answerId",
                table: "voteAnswers",
                column: "answerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
