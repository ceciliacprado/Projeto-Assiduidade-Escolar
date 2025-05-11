using API.Models;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/disciplina")]
public class DisciplinaController : ControllerBase
{
    private readonly IDisciplinaRepository _disciplinaRepository;
    private readonly IAlunoRepository _alunoRepository;

    public DisciplinaController(IDisciplinaRepository disciplinaRepository, IAlunoRepository alunoRepository)
    {
        _disciplinaRepository = disciplinaRepository;
        _alunoRepository = alunoRepository;
    }

    [HttpPost("cadastrar")]
    [Authorize(Roles = "admin")]
    public IActionResult Cadastrar([FromBody] Disciplina disciplina)
    {
        _disciplinaRepository.Cadastrar(disciplina);
        return Created("", disciplina);
    }

    [HttpGet("listar")]
    [Authorize]
    public IActionResult Listar()
    {
        return Ok(_disciplinaRepository.Listar());
    }

    [HttpGet("{id}")]
    [Authorize]
    public IActionResult BuscarPorId(int id)
    {
        var disciplina = _disciplinaRepository.BuscarPorId(id);
        if (disciplina == null)
        {
            return NotFound(new { mensagem = "Disciplina não encontrada" });
        }
        return Ok(disciplina);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public IActionResult Atualizar(int id, [FromBody] Disciplina disciplina)
    {
        var disciplinaExistente = _disciplinaRepository.BuscarPorId(id);
        if (disciplinaExistente == null)
        {
            return NotFound(new { mensagem = "Disciplina não encontrada" });
        }

        disciplinaExistente.Nome = disciplina.Nome;
        _disciplinaRepository.Atualizar(disciplinaExistente);
        return Ok(disciplinaExistente);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public IActionResult Deletar(int id)
    {
        var disciplina = _disciplinaRepository.BuscarPorId(id);
        if (disciplina == null)
        {
            return NotFound(new { mensagem = "Disciplina não encontrada" });
        }

        // Verifica se existem alunos vinculados à disciplina
        var alunosVinculados = _alunoRepository.Listar().Any(a => a.DisciplinaId == id);
        if (alunosVinculados)
        {
            return BadRequest(new { mensagem = "Não é possível deletar a disciplina pois existem alunos vinculados a ela" });
        }

        _disciplinaRepository.Deletar(id);
        return NoContent();
    }
}