using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class FrequenciaRepository : IFrequenciaRepository
    {
        private readonly AppDataContext _context;

        public FrequenciaRepository(AppDataContext context)
        {
            _context = context;
        }

        public void Registrar(Frequencia frequencia)
        {
            _context.Frequencias.Add(frequencia);
            _context.SaveChanges();
        }

        public List<Frequencia> ObterFaltasPorAluno(int alunoId)
        {
            return _context.Frequencias
                .Where(f => f.AlunoId == alunoId && !f.Presente)
                .ToList();
        }

        public int ObterFaltasPorDisciplina(int alunoId, int disciplinaId)
        {
            return _context.Frequencias
                .Count(f => f.AlunoId == alunoId && f.DisciplinaId == disciplinaId && !f.Presente);
        }
    }
}
