using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Professor
    {
        public int Id { get; set; }
        
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        public string Senha { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "Professor";
        
        [StringLength(100)]
        public string Especialidade { get; set; } = string.Empty;
        
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        
        // Relacionamento com Disciplinas
        public ICollection<Disciplina> Disciplinas { get; set; } = new List<Disciplina>();
    }
}