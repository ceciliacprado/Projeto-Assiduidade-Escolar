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
    public async Task<IActionResult> Cadastrar([FromBody] Disciplina disciplina)
    {
        try
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

            // Verificar se a turma existe
            var turma = await _context.Turmas.FindAsync(disciplina.TurmaId);
            if (turma == null)
            {
                return BadRequest(new { mensagem = "Turma não encontrada" });
            }

            // Cadastrar a disciplina
            _context.Disciplinas.Add(disciplina);
            await _context.SaveChangesAsync();

            // Vincular automaticamente todos os alunos da turma à nova disciplina
            var alunosDaTurma = await _context.Alunos
                .Where(a => a.TurmaId == disciplina.TurmaId)
                .ToListAsync();

            var dataAtual = DateTime.Now.Date;
            var frequencias = new List<Frequencia>();

            foreach (var aluno in alunosDaTurma)
            {
                // Verificar se já existe registro de frequência para este aluno nesta disciplina nesta data
                var frequenciaExistente = await _context.Frequencias
                    .FirstOrDefaultAsync(f => f.AlunoId == aluno.Id && 
                                            f.DisciplinaId == disciplina.Id && 
                                            f.Data.Date == dataAtual);
                
                if (frequenciaExistente == null)
                {
                    frequencias.Add(new Frequencia
                    {
                        AlunoId = aluno.Id,
                        DisciplinaId = disciplina.Id,
                        Data = dataAtual,
                        Presente = false // Inicialmente como ausente, será atualizado quando o professor fizer a chamada
                    });
                }
            }

            if (frequencias.Any())
            {
                _context.Frequencias.AddRange(frequencias);
                await _context.SaveChangesAsync();
            }

            // Retornar a disciplina com informações adicionais
            var disciplinaCadastrada = await _context.Disciplinas
                .Include(d => d.Turma)
                .Include(d => d.Professor)
                .FirstAsync(d => d.Id == disciplina.Id);

            var resultado = new
            {
                disciplina = disciplinaCadastrada,
                alunosVinculados = alunosDaTurma.Count,
                mensagem = $"Disciplina cadastrada com sucesso. {alunosDaTurma.Count} alunos foram vinculados automaticamente."
            };

            return Created("", resultado);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao cadastrar disciplina: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
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
    public async Task<IActionResult> Atualizar(int id, [FromBody] Disciplina disciplina)
    {
        try
        {
            var disciplinaExistente = await _context.Disciplinas.FindAsync(id);
            if (disciplinaExistente == null)
            {
                return NotFound(new { mensagem = "Disciplina não encontrada" });
            }

            // Verificar se a nova turma existe
            var turma = await _context.Turmas.FindAsync(disciplina.TurmaId);
            if (turma == null)
            {
                return BadRequest(new { mensagem = "Turma não encontrada" });
            }

            // Se a turma mudou, remover frequências antigas e criar novas
            if (disciplina.TurmaId != disciplinaExistente.TurmaId)
            {
                // Remover frequências antigas desta disciplina
                var frequenciasAntigas = await _context.Frequencias
                    .Where(f => f.DisciplinaId == id)
                    .ToListAsync();
                
                _context.Frequencias.RemoveRange(frequenciasAntigas);

                // Criar novas frequências para alunos da nova turma
                var alunosDaNovaTurma = await _context.Alunos
                    .Where(a => a.TurmaId == disciplina.TurmaId)
                    .ToListAsync();

                var dataAtual = DateTime.Now.Date;
                var novasFrequencias = new List<Frequencia>();

                foreach (var aluno in alunosDaNovaTurma)
                {
                    novasFrequencias.Add(new Frequencia
                    {
                        AlunoId = aluno.Id,
                        DisciplinaId = id,
                        Data = dataAtual,
                        Presente = false
                    });
                }

                if (novasFrequencias.Any())
                {
                    _context.Frequencias.AddRange(novasFrequencias);
                }
            }

            // Atualizar dados da disciplina
            disciplinaExistente.Nome = disciplina.Nome;
            disciplinaExistente.Codigo = disciplina.Codigo;
            disciplinaExistente.CargaHoraria = disciplina.CargaHoraria;
            disciplinaExistente.TurmaId = disciplina.TurmaId;
            disciplinaExistente.ProfessorId = disciplina.ProfessorId;

            await _context.SaveChangesAsync();

            // Retornar a disciplina atualizada
            var disciplinaAtualizada = await _context.Disciplinas
                .Include(d => d.Turma)
                .Include(d => d.Professor)
                .FirstAsync(d => d.Id == id);

            return Ok(disciplinaAtualizada);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar disciplina: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Excluir(int id)
    {
        try
        {
            var disciplina = await _context.Disciplinas.FindAsync(id);
            if (disciplina == null)
            {
                return NotFound(new { mensagem = "Disciplina não encontrada" });
            }

            // Remover frequências relacionadas à disciplina
            var frequencias = await _context.Frequencias
                .Where(f => f.DisciplinaId == id)
                .ToListAsync();
            
            _context.Frequencias.RemoveRange(frequencias);
            _context.Disciplinas.Remove(disciplina);
            await _context.SaveChangesAsync();
            
            return Ok(new { mensagem = "Disciplina excluída com sucesso" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao excluir disciplina: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
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

    [HttpPost("vincular-alunos-existente/{disciplinaId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> VincularAlunosExistente(int disciplinaId)
    {
        try
        {
            Console.WriteLine($"Iniciando vinculação de alunos à disciplina {disciplinaId}");
            
            var disciplina = await _context.Disciplinas
                .Include(d => d.Turma)
                .FirstOrDefaultAsync(d => d.Id == disciplinaId);
            
            if (disciplina == null)
            {
                return NotFound(new { mensagem = "Disciplina não encontrada" });
            }
            
            var alunosDaTurma = await _context.Alunos
                .Where(a => a.TurmaId == disciplina.TurmaId)
                .ToListAsync();
            
            var totalVinculados = 0;
            var dataAtual = DateTime.Now.Date;
            
            foreach (var aluno in alunosDaTurma)
            {
                // Verificar se já existe registro de frequência
                var frequenciaExistente = await _context.Frequencias
                    .FirstOrDefaultAsync(f => f.AlunoId == aluno.Id && 
                                            f.DisciplinaId == disciplinaId && 
                                            f.Data.Date == dataAtual);
                
                if (frequenciaExistente == null)
                {
                    _context.Frequencias.Add(new Frequencia
                    {
                        AlunoId = aluno.Id,
                        DisciplinaId = disciplinaId,
                        Data = dataAtual,
                        Presente = false
                    });
                    totalVinculados++;
                }
            }
            
            await _context.SaveChangesAsync();
            
            Console.WriteLine($"Vinculação concluída: {totalVinculados} alunos vinculados à disciplina {disciplina.Nome}");
            return Ok(new { 
                mensagem = "Vinculação concluída com sucesso", 
                disciplina = disciplina.Nome,
                totalVinculados = totalVinculados,
                totalAlunos = alunosDaTurma.Count
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro na vinculação: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
    }
}