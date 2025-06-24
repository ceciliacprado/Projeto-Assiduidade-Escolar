using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Disciplina
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Código é obrigatório")]
        [StringLength(20, ErrorMessage = "Código deve ter no máximo 20 caracteres")]
        public string Codigo { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Carga horária é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "Carga horária deve ser maior que zero")]
        public int CargaHoraria { get; set; }
        
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        
        // Relacionamento com Turma (obrigatório)
        [Required(ErrorMessage = "Turma é obrigatória")]
        public int TurmaId { get; set; }
        public Turma Turma { get; set; } = null!;
        
        // Relacionamento com Professor (opcional)
        public int? ProfessorId { get; set; }
        public Professor? Professor { get; set; }
        
        // Relacionamento many-to-many com Alunos
        public ICollection<Aluno> Alunos { get; set; } = new List<Aluno>();
        
        // Relacionamento com Frequencias
        public ICollection<Frequencia> Frequencias { get; set; } = new List<Frequencia>();
    }
}