using System;
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
            // Validações iniciais
            if (frequencia == null)
                throw new ArgumentNullException(nameof(frequencia));

            if (frequencia.AlunoId <= 0)
                throw new ArgumentException("AlunoId inválido", nameof(frequencia));

            if (frequencia.DisciplinaId <= 0)
                throw new ArgumentException("DisciplinaId inválido", nameof(frequencia));

            // Verifica se já existe registro para a mesma data
            var registroExistente = BuscarPorDataEAluno(frequencia.Data, frequencia.AlunoId, frequencia.DisciplinaId);
            if (registroExistente != null)
                throw new InvalidOperationException("Já existe um registro de frequência para este aluno nesta data");

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // Adiciona a frequência
                _context.Frequencias.Add(frequencia);

                // Tenta salvar as alterações
                var result = _context.SaveChanges();

                // Se não salvou nenhum registro, lança exceção
                if (result == 0)
                    throw new DbUpdateException("Falha ao salvar o registro de frequência", null);

                // Se chegou aqui, commit da transação
                transaction.Commit();
            }
            catch (Exception)
            {
                // Em caso de erro, faz rollback
                transaction.Rollback();
                throw;
            }
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

        public Frequencia? BuscarPorDataEAluno(DateTime data, int alunoId, int disciplinaId)
        {
            return _context.Frequencias
                .FirstOrDefault(f => f.Data.Date == data.Date &&
                                   f.AlunoId == alunoId &&
                                   f.DisciplinaId == disciplinaId);
        }
    }
}
