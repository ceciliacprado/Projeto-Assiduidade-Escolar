using API.Models;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/frequencia")]
public class FrequenciaController : ControllerBase
{
    private readonly IFrequenciaRepository _frequenciaRepository;
    private readonly IAlunoRepository _alunoRepository;
    private readonly IDisciplinaRepository _disciplinaRepository;

    public FrequenciaController(
        IFrequenciaRepository frequenciaRepository,
        IAlunoRepository alunoRepository,
        IDisciplinaRepository disciplinaRepository)
    {
        _frequenciaRepository = frequenciaRepository;
        _alunoRepository = alunoRepository;
        _disciplinaRepository = disciplinaRepository;
    }

    [HttpPost("registrar")]
    [Authorize(Roles = "admin")]
    public IActionResult RegistrarFrequencia([FromBody] Frequencia frequencia)
    {
        // Verifica se o aluno existe
        var aluno = _alunoRepository.BuscarPorId(frequencia.AlunoId);
        if (aluno == null)
        {
            return NotFound(new { mensagem = "Aluno não encontrado" });
        }

        // Verifica se a disciplina existe
        var disciplina = _disciplinaRepository.BuscarPorId(frequencia.DisciplinaId);
        if (disciplina == null)
        {
            return NotFound(new { mensagem = "Disciplina não encontrada" });
        }

        // Verifica se o aluno está matriculado na disciplina
        if (aluno.DisciplinaId != frequencia.DisciplinaId)
        {
            return BadRequest(new { mensagem = "Aluno não está matriculado nesta disciplina" });
        }

        // Verifica se já existe registro de frequência para este aluno nesta data
        var frequenciaExistente = _frequenciaRepository.BuscarPorDataEAluno(
            frequencia.Data.Date,
            frequencia.AlunoId,
            frequencia.DisciplinaId
        );

        if (frequenciaExistente != null)
        {
            return BadRequest(new { mensagem = "Já existe registro de frequência para este aluno nesta data" });
        }

        _frequenciaRepository.Registrar(frequencia);
        return Created("", frequencia);
    }

    [HttpPost("faltas-aluno")]
    [Authorize]
    public IActionResult ObterFaltasPorAluno([FromBody] ConsultaFaltasRequest request)
    {
        var aluno = _alunoRepository.BuscarPorId(request.AlunoId);
        if (aluno == null)
        {
            return NotFound(new { mensagem = "Aluno não encontrado" });
        }

        var faltas = _frequenciaRepository.ObterFaltasPorAluno(request.AlunoId);
        return Ok(faltas);
    }

    [HttpPost("faltas-disciplina")]
    [Authorize]
    public IActionResult ObterFaltasPorDisciplina([FromBody] ConsultaFaltasRequest request)
    {
        var aluno = _alunoRepository.BuscarPorId(request.AlunoId);
        if (aluno == null)
        {
            return NotFound(new { mensagem = "Aluno não encontrado" });
        }

        var disciplina = _disciplinaRepository.BuscarPorId(request.DisciplinaId);
        if (disciplina == null)
        {
            return NotFound(new { mensagem = "Disciplina não encontrada" });
        }

        var faltas = _frequenciaRepository.ObterFaltasPorDisciplina(request.AlunoId, request.DisciplinaId);
        return Ok(new { totalFaltas = faltas });
    }

    [HttpPost("situacao")]
    [Authorize]
    public IActionResult ObterSituacaoAluno([FromBody] ConsultaFaltasRequest request)
    {
        var aluno = _alunoRepository.BuscarPorId(request.AlunoId);
        if (aluno == null)
        {
            return NotFound(new { mensagem = "Aluno não encontrado" });
        }

        var disciplina = _disciplinaRepository.BuscarPorId(request.DisciplinaId);
        if (disciplina == null)
        {
            return NotFound(new { mensagem = "Disciplina não encontrada" });
        }

        var faltas = _frequenciaRepository.ObterFaltasPorDisciplina(request.AlunoId, request.DisciplinaId);
        return Ok(new {
            alunoId = request.AlunoId,
            disciplinaId = request.DisciplinaId,
            totalFaltas = faltas
        });
    }
}

public class ConsultaFaltasRequest
{
    public int AlunoId { get; set; }
    public int DisciplinaId { get; set; }
}