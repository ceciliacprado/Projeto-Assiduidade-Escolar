using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Repositories;

namespace API.Controllers;

[ApiController]
[Route("api/disciplina")]
public class DisciplinaController : ControllerBase
{
    private readonly AppDataContext _context;
    private readonly IAlunoRepository _alunoRepository;

    public DisciplinaController(AppDataContext context, IAlunoRepository alunoRepository)
    {
        _context = context;
        _alunoRepository = alunoRepository;
    }

    [HttpPost("cadastrar")]
    [Authorize(Roles = "admin")]
    public IActionResult Cadastrar([FromBody] Disciplina disciplina)
    {
        if (string.IsNullOrWhiteSpace(disciplina.Nome))
        {
            return BadRequest(new { mensagem = "Nome é obrigatório" });
        }

        if (string.IsNullOrWhiteSpace(disciplina.Codigo))
        {
            return BadRequest(new { mensagem = "Código é obrigatório" });
        }

        if (disciplina.CargaHoraria <= 0)
        {
            return BadRequest(new { mensagem = "Carga horária deve ser maior que zero" });
        }

        // Verificar se código já existe
        if (_context.Disciplinas.Any(d => d.Codigo == disciplina.Codigo))
        {
            return BadRequest(new { mensagem = "Código já cadastrado" });
        }

        _context.Disciplinas.Add(disciplina);
        _context.SaveChanges();
        return Created("", disciplina);
    }

    [HttpGet("listar")]
    [Authorize]
    public IActionResult Listar()
    {
        var disciplinas = _context.Disciplinas
            .Include(d => d.Turma)
            .Include(d => d.Professor)
            .ToList();
        return Ok(disciplinas);
    }

    [HttpGet("{id}")]
    [Authorize]
    public IActionResult BuscarPorId(int id)
    {
        var disciplina = _context.Disciplinas
            .Include(d => d.Turma)
            .Include(d => d.Professor)
            .FirstOrDefault(d => d.Id == id);

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
        var disciplinaExistente = _context.Disciplinas.Find(id);
        if (disciplinaExistente == null)
        {
            return NotFound(new { mensagem = "Disciplina não encontrada" });
        }

        disciplinaExistente.Nome = disciplina.Nome;
        disciplinaExistente.Codigo = disciplina.Codigo;
        disciplinaExistente.CargaHoraria = disciplina.CargaHoraria;
        disciplinaExistente.TurmaId = disciplina.TurmaId;
        disciplinaExistente.ProfessorId = disciplina.ProfessorId;

        _context.SaveChanges();
        return Ok(disciplinaExistente);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public IActionResult Excluir(int id)
    {
        var disciplina = _context.Disciplinas
            .Include(d => d.Alunos)
            .FirstOrDefault(d => d.Id == id);
            
        if (disciplina == null)
        {
            return NotFound(new { mensagem = "Disciplina não encontrada" });
        }

        // Verifica se existem alunos vinculados à disciplina
        if (disciplina.Alunos.Any())
        {
            return BadRequest(new { mensagem = "Não é possível deletar a disciplina pois existem alunos vinculados a ela" });
        }

        _context.Disciplinas.Remove(disciplina);
        _context.SaveChanges();
        return Ok(new { mensagem = "Disciplina excluída com sucesso" });
    }

    [HttpGet("turma/{turmaId}")]
    public IActionResult ListarPorTurma(int turmaId)
    {
        var disciplinas = _context.Disciplinas
            .Include(d => d.Turma)
            .Include(d => d.Professor)
            .Where(d => d.TurmaId == turmaId)
            .ToList();

        return Ok(disciplinas);
    }

    [HttpGet("professor/{professorId}")]
    public IActionResult ListarPorProfessor(int professorId)
    {
        var disciplinas = _context.Disciplinas
            .Include(d => d.Turma)
            .Include(d => d.Professor)
            .Where(d => d.ProfessorId == professorId)
            .ToList();

        return Ok(disciplinas);
    }
}