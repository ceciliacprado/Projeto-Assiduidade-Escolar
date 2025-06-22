using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace API.Models
{
    public class Turma
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Nome { get; set; } = string.Empty;
        
        [StringLength(10)]
        public string Ano { get; set; } = string.Empty;
        
        [StringLength(5)]
        public string Serie { get; set; } = string.Empty;
        
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        
        // Relacionamento com Alunos
        public ICollection<Aluno> Alunos { get; set; } = new List<Aluno>();
        
        // Relacionamento com Disciplinas
        public ICollection<Disciplina> Disciplinas { get; set; } = new List<Disciplina>();
    }
} 