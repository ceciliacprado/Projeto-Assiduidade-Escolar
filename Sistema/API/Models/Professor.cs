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
        
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}