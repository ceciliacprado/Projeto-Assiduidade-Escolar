using System;
using API.Models;

namespace API.Repositories;

public interface IFrequenciaRepository
{
    void Registrar(Frequencia frequencia);
    List<Frequencia> ObterFaltasPorAluno(int alunoId);
    int ObterFaltasPorDisciplina(int alunoId, int disciplinaId);
    Frequencia? BuscarPorDataEAluno(DateTime data, int alunoId, int disciplinaId);
}
