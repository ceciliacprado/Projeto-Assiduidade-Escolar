using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Professor
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(150, ErrorMessage = "Email deve ter no máximo 150 caracteres")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Senha é obrigatória")]
        [StringLength(255, ErrorMessage = "Senha deve ter no máximo 255 caracteres")]
        public string Senha { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Especialidade é obrigatória")]
        [StringLength(100, ErrorMessage = "Especialidade deve ter no máximo 100 caracteres")]
        public string Especialidade { get; set; } = string.Empty;
        
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        
        // Role fixa como "admin" - não pode ser alterada
        public string Role => "admin";
        
        // Relacionamento com Disciplinas
        public ICollection<Disciplina> Disciplinas { get; set; } = new List<Disciplina>();
    }
}