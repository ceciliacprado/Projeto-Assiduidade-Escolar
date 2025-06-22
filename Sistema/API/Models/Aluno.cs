using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class Aluno
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public int? TurmaId { get; set; }
    public Turma? Turma { get; set; }
    
    // Relacionamento com Disciplina (opcional)
    public int? DisciplinaId { get; set; }
    public Disciplina? Disciplina { get; set; }
    
    // Relacionamento com Frequencias
    public ICollection<Frequencia> Frequencias { get; set; } = new List<Frequencia>();
}
