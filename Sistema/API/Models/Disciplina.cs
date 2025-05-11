using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Disciplina
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public List<Frequencia>? Frequencias { get; set; }
}
}