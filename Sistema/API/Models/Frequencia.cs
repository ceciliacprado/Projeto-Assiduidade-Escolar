using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Frequencia
    {
        public int Id { get; set; }
        
        [Required]
        public int AlunoId { get; set; }
        public Aluno? Aluno { get; set; }
        
        [Required]
        public int DisciplinaId { get; set; }
        public Disciplina? Disciplina { get; set; }
        
        [Required]
        public DateTime Data { get; set; } = DateTime.UtcNow;
        
        [Required]
        public bool Presente { get; set; }
        
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}