using System;

namespace API.Models;

public class Aluno
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }

    public int DisciplinaId { get; set; }
    public Disciplina? Disciplina { get; set; }
}
