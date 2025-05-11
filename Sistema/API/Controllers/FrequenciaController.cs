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

    public FrequenciaController(IFrequenciaRepository frequenciaRepository)
    {
        _frequenciaRepository = frequenciaRepository;
    }

    [HttpPost("registrar")]
    [Authorize(Roles = "admin")]
    public IActionResult RegistrarFrequencia([FromBody] Frequencia frequencia)
    {
        _frequenciaRepository.Registrar(frequencia);
        return Created("", frequencia);
    }

    [HttpGet("listar/{alunoId}")]
    [Authorize]
    public IActionResult ListarPorAluno(int alunoId)
    {
        var faltas = _frequenciaRepository.ObterFaltasPorAluno(alunoId);
        return Ok(faltas);
    }

    [HttpGet("situacao/{alunoId}/{disciplinaId}")]
    [Authorize]
    public IActionResult VerificarSituacao(int alunoId, int disciplinaId)
    {
        int faltas = _frequenciaRepository.ObterFaltasPorDisciplina(alunoId, disciplinaId);
        string situacao = faltas > 10 ? "Reprovado por faltas" : "Aprovado por frequência";
        return Ok(new { alunoId, disciplinaId, faltas, situacao });
    }
}