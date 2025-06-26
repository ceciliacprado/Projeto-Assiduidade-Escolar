using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class Aluno
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string Nome { get; set; } = string.Empty;
    
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    // TurmaId obrigatório
    [Required(ErrorMessage = "Turma é obrigatória")]
    public int TurmaId { get; set; }
    public Turma Turma { get; set; } = null!;
    
    // Relacionamento com Frequencias
    public ICollection<Frequencia> Frequencias { get; set; } = new List<Frequencia>();
}
