using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Frequencia
{
    public int Id { get; set; }
    public int AlunoId { get; set; }
    public Aluno Aluno { get; set; }
    
    public int DisciplinaId { get; set; }
    public Disciplina Disciplina { get; set; }
    
    public DateTime Data { get; set; }
    public bool Presente { get; set; }
}
}