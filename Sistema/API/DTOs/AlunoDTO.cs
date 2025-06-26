using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class AlunoDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
    public int TurmaId { get; set; }
    public TurmaDTO Turma { get; set; } = null!;
}

public class TurmaDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Ano { get; set; } = string.Empty;
    public string Serie { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}

public class CadastrarAlunoDTO
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string Nome { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Turma é obrigatória")]
    public int TurmaId { get; set; }
}

public class AtualizarAlunoDTO
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string Nome { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Turma é obrigatória")]
    public int TurmaId { get; set; }
} 