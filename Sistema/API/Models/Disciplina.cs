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
        
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = string.Empty;
        
        [Required]
        public int CargaHoraria { get; set; }
        
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        
        // Relacionamento com Turma
        public int? TurmaId { get; set; }
        public Turma? Turma { get; set; }
        
        // Relacionamento com Professor
        public int? ProfessorId { get; set; }
        public Professor? Professor { get; set; }
        
        // Relacionamento com Alunos
        public ICollection<Aluno> Alunos { get; set; } = new List<Aluno>();
        
        // Relacionamento com Frequencias
        public ICollection<Frequencia> Frequencias { get; set; } = new List<Frequencia>();
    }
}