using API.Data;
using API.Models;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/aluno")]
public class AlunoController : ControllerBase
{
    private readonly AppDataContext _context;

    public AlunoController(AppDataContext context)
    {
        _context = context;
    }

    [HttpGet("listar")]
    [Authorize]
    public async Task<IActionResult> Listar()
    {
        try
        {
            Console.WriteLine("Endpoint /api/aluno/listar foi chamado");
            
            var alunos = await _context.Alunos
                .Include(a => a.Turma)
                .Select(a => new AlunoDTO
                {
                    Id = a.Id,
                    Nome = a.Nome,
                    CriadoEm = a.CriadoEm,
                    TurmaId = a.TurmaId,
                    Turma = new TurmaDTO
                    {
                        Id = a.Turma.Id,
                        Nome = a.Turma.Nome,
                        Ano = a.Turma.Ano,
                        Serie = a.Turma.Serie,
                        CriadoEm = a.Turma.CriadoEm
                    }
                })
                .ToListAsync();
                
            Console.WriteLine($"Total de alunos encontrados: {alunos.Count}");
            return Ok(alunos);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no endpoint Listar: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        try
        {
            Console.WriteLine($"Endpoint /api/aluno/{id} foi chamado");
            
            var aluno = await _context.Alunos
                .Include(a => a.Turma)
                .Where(a => a.Id == id)
                .Select(a => new AlunoDTO
                {
                    Id = a.Id,
                    Nome = a.Nome,
                    CriadoEm = a.CriadoEm,
                    TurmaId = a.TurmaId,
                    Turma = new TurmaDTO
                    {
                        Id = a.Turma.Id,
                        Nome = a.Turma.Nome,
                        Ano = a.Turma.Ano,
                        Serie = a.Turma.Serie,
                        CriadoEm = a.Turma.CriadoEm
                    }
                })
                .FirstOrDefaultAsync();
                
            if (aluno == null)
            {
                Console.WriteLine($"Aluno com ID {id} não encontrado");
                return NotFound(new { mensagem = "Aluno não encontrado" });
            }
            
            Console.WriteLine($"Aluno encontrado: {aluno.Nome}");
            return Ok(aluno);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no endpoint BuscarPorId: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
    }

    [HttpPost("cadastrar")]
    [Authorize]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarAlunoDTO dto)
    {
        try
        {
            Console.WriteLine($"Endpoint /api/aluno/cadastrar foi chamado para: {dto.Nome}");
            
            // Validar se a turma existe
            var turma = await _context.Turmas.FindAsync(dto.TurmaId);
            if (turma == null)
            {
                Console.WriteLine($"Turma com ID {dto.TurmaId} não encontrada");
                return BadRequest(new { mensagem = "Turma não encontrada" });
            }
            
            // Validar dados obrigatórios
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                return BadRequest(new { mensagem = "Nome do aluno é obrigatório" });
            }
            
            // Cadastrar o aluno
            var aluno = new Aluno
            {
                Nome = dto.Nome,
                TurmaId = dto.TurmaId
            };
            
            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();
            
            // Retornar o aluno com a turma incluída
            var alunoCadastrado = await _context.Alunos
                .Include(a => a.Turma)
                .Where(a => a.Id == aluno.Id)
                .Select(a => new AlunoDTO
                {
                    Id = a.Id,
                    Nome = a.Nome,
                    CriadoEm = a.CriadoEm,
                    TurmaId = a.TurmaId,
                    Turma = new TurmaDTO
                    {
                        Id = a.Turma.Id,
                        Nome = a.Turma.Nome,
                        Ano = a.Turma.Ano,
                        Serie = a.Turma.Serie,
                        CriadoEm = a.Turma.CriadoEm
                    }
                })
                .FirstAsync();
                
            Console.WriteLine($"Aluno cadastrado com sucesso: {alunoCadastrado.Nome}");
            return Created("", alunoCadastrado);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no endpoint Cadastrar: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarAlunoDTO dto)
    {
        try
        {
            Console.WriteLine($"Endpoint /api/aluno/{id} (PUT) foi chamado");
            
            var alunoExistente = await _context.Alunos.FindAsync(id);
            if (alunoExistente == null)
            {
                Console.WriteLine($"Aluno com ID {id} não encontrado");
                return NotFound(new { mensagem = "Aluno não encontrado" });
            }
            
            // Validar se a nova turma existe
            if (dto.TurmaId != alunoExistente.TurmaId)
            {
                var turma = await _context.Turmas.FindAsync(dto.TurmaId);
                if (turma == null)
                {
                    Console.WriteLine($"Turma com ID {dto.TurmaId} não encontrada");
                    return BadRequest(new { mensagem = "Turma não encontrada" });
                }
            }
            
            // Validar dados obrigatórios
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                return BadRequest(new { mensagem = "Nome do aluno é obrigatório" });
            }
            
            // Atualizar dados
            alunoExistente.Nome = dto.Nome;
            alunoExistente.TurmaId = dto.TurmaId;
            
            await _context.SaveChangesAsync();
            
            // Retornar o aluno atualizado com a turma incluída
            var alunoAtualizado = await _context.Alunos
                .Include(a => a.Turma)
                .Where(a => a.Id == id)
                .Select(a => new AlunoDTO
                {
                    Id = a.Id,
                    Nome = a.Nome,
                    CriadoEm = a.CriadoEm,
                    TurmaId = a.TurmaId,
                    Turma = new TurmaDTO
                    {
                        Id = a.Turma.Id,
                        Nome = a.Turma.Nome,
                        Ano = a.Turma.Ano,
                        Serie = a.Turma.Serie,
                        CriadoEm = a.Turma.CriadoEm
                    }
                })
                .FirstAsync();
                
            Console.WriteLine($"Aluno atualizado com sucesso: {alunoAtualizado.Nome}");
            return Ok(alunoAtualizado);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no endpoint Atualizar: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Excluir(int id)
    {
        try
        {
            Console.WriteLine($"Endpoint /api/aluno/{id} (DELETE) foi chamado");
            
            var aluno = await _context.Alunos.FindAsync(id);
            if (aluno == null)
            {
                Console.WriteLine($"Aluno com ID {id} não encontrado");
                return NotFound(new { mensagem = "Aluno não encontrado" });
            }
            
            _context.Alunos.Remove(aluno);
            await _context.SaveChangesAsync();
            
            Console.WriteLine($"Aluno excluído com sucesso: {aluno.Nome}");
            return Ok(new { mensagem = "Aluno excluído com sucesso" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no endpoint Excluir: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
    }

    [HttpGet("turma/{turmaId}")]
    [Authorize]
    public async Task<IActionResult> ListarPorTurma(int turmaId)
    {
        try
        {
            Console.WriteLine($"Endpoint /api/aluno/turma/{turmaId} foi chamado");
            
            // Verificar se a turma existe
            var turma = await _context.Turmas.FindAsync(turmaId);
            if (turma == null)
            {
                Console.WriteLine($"Turma com ID {turmaId} não encontrada");
                return NotFound(new { mensagem = "Turma não encontrada" });
            }
            
            var alunos = await _context.Alunos
                .Include(a => a.Turma)
                .Where(a => a.TurmaId == turmaId)
                .Select(a => new AlunoDTO
                {
                    Id = a.Id,
                    Nome = a.Nome,
                    CriadoEm = a.CriadoEm,
                    TurmaId = a.TurmaId,
                    Turma = new TurmaDTO
                    {
                        Id = a.Turma.Id,
                        Nome = a.Turma.Nome,
                        Ano = a.Turma.Ano,
                        Serie = a.Turma.Serie,
                        CriadoEm = a.Turma.CriadoEm
                    }
                })
                .ToListAsync();
                
            Console.WriteLine($"Alunos encontrados para turma {turma.Nome}: {alunos.Count}");
            return Ok(alunos);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no endpoint ListarPorTurma: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
    }

    [HttpGet("teste-banco")]
    public async Task<IActionResult> TesteBanco()
    {
        try
        {
            Console.WriteLine("Testando conexão com banco...");
            
            var totalAlunos = await _context.Alunos.CountAsync();
            var totalTurmas = await _context.Turmas.CountAsync();
            
            Console.WriteLine($"Total de alunos: {totalAlunos}");
            Console.WriteLine($"Total de turmas: {totalTurmas}");
            
            return Ok(new
            {
                totalAlunos,
                totalTurmas,
                mensagem = "Conexão com banco OK"
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no teste do banco: {ex.Message}");
            return StatusCode(500, new { mensagem = $"Erro no banco: {ex.Message}" });
        }
    }
}