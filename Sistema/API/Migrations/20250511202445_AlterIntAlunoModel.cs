using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AlterIntAlunoModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alunos_Disciplinas_DisciplinaId",
                table: "Alunos");

            migrationBuilder.AlterColumn<int>(
                name: "DisciplinaId",
                table: "Alunos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Alunos_Disciplinas_DisciplinaId",
                table: "Alunos",
                column: "DisciplinaId",
                principalTable: "Disciplinas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alunos_Disciplinas_DisciplinaId",
                table: "Alunos");

            migrationBuilder.AlterColumn<int>(
                name: "DisciplinaId",
                table: "Alunos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Alunos_Disciplinas_DisciplinaId",
                table: "Alunos",
                column: "DisciplinaId",
                principalTable: "Disciplinas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
