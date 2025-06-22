using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/turma")]
public class TurmaController : ControllerBase
{
    private readonly AppDataContext _context;

    public TurmaController(AppDataContext context)
    {
        _context = context;
    }

    [HttpGet("listar")]
    public IActionResult Listar()
    {
        var turmas = _context.Turmas.ToList();
        return Ok(turmas);
    }

    [HttpGet("{id}")]
    public IActionResult BuscarPorId(int id)
    {
        var turma = _context.Turmas.Find(id);
        if (turma == null)
        {
            return NotFound(new { mensagem = "Turma não encontrada" });
        }
        return Ok(turma);
    }

    [HttpPost("cadastrar")]
    public IActionResult Cadastrar([FromBody] Turma turma)
    {
        if (string.IsNullOrWhiteSpace(turma.Nome))
        {
            return BadRequest(new { mensagem = "Nome da turma é obrigatório" });
        }

        _context.Turmas.Add(turma);
        _context.SaveChanges();
        return Created("", turma);
    }

    [HttpPut("{id}")]
    public IActionResult Atualizar(int id, [FromBody] Turma turma)
    {
        var turmaExistente = _context.Turmas.Find(id);
        if (turmaExistente == null)
        {
            return NotFound(new { mensagem = "Turma não encontrada" });
        }

        turmaExistente.Nome = turma.Nome;
        turmaExistente.Ano = turma.Ano;
        turmaExistente.Serie = turma.Serie;

        _context.SaveChanges();
        return Ok(turmaExistente);
    }

    [HttpDelete("{id}")]
    public IActionResult Excluir(int id)
    {
        var turma = _context.Turmas.Find(id);
        if (turma == null)
        {
            return NotFound(new { mensagem = "Turma não encontrada" });
        }

        _context.Turmas.Remove(turma);
        _context.SaveChanges();
        return Ok(new { mensagem = "Turma excluída com sucesso" });
    }

    [HttpGet("{id}/alunos")]
    public IActionResult ListarAlunos(int id)
    {
        var turma = _context.Turmas
            .Include(t => t.Alunos)
            .FirstOrDefault(t => t.Id == id);

        if (turma == null)
        {
            return NotFound(new { mensagem = "Turma não encontrada" });
        }

        return Ok(turma.Alunos);
    }

    [HttpGet("{id}/disciplinas")]
    public IActionResult ListarDisciplinas(int id)
    {
        var turma = _context.Turmas
            .Include(t => t.Disciplinas)
            .FirstOrDefault(t => t.Id == id);

        if (turma == null)
        {
            return NotFound(new { mensagem = "Turma não encontrada" });
        }

        return Ok(turma.Disciplinas);
    }
} 