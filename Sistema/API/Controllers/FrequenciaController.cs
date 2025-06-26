using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Repositories;

namespace API.Controllers;

[ApiController]
[Route("api/frequencia")]
public class FrequenciaController : ControllerBase
{
    private readonly AppDataContext _context;
    private readonly IAlunoRepository _alunoRepository;
    private readonly IDisciplinaRepository _disciplinaRepository;

    public FrequenciaController(
        AppDataContext context,
        IAlunoRepository alunoRepository,
        IDisciplinaRepository disciplinaRepository)
    {
        _context = context;
        _alunoRepository = alunoRepository;
        _disciplinaRepository = disciplinaRepository;
    }

    [HttpGet("listar")]
    public IActionResult Listar()
    {
        var frequencias = _context.Frequencias
            .Include(f => f.Aluno)
            .Include(f => f.Disciplina)
            .ToList();
        return Ok(frequencias);
    }

    [HttpGet("{id}")]
    public IActionResult BuscarPorId(int id)
    {
        var frequencia = _context.Frequencias
            .Include(f => f.Aluno)
            .Include(f => f.Disciplina)
            .FirstOrDefault(f => f.Id == id);

        if (frequencia == null)
        {
            return NotFound(new { mensagem = "Frequência não encontrada" });
        }
        return Ok(frequencia);
    }

    [HttpPost("cadastrar")]
    public IActionResult Cadastrar([FromBody] Frequencia frequencia)
    {
        if (frequencia.AlunoId <= 0)
        {
            return BadRequest(new { mensagem = "Aluno é obrigatório" });
        }

        if (frequencia.DisciplinaId <= 0)
        {
            return BadRequest(new { mensagem = "Disciplina é obrigatória" });
        }

        // Verificar se aluno existe
        var aluno = _context.Alunos.Find(frequencia.AlunoId);
        if (aluno == null)
        {
            return BadRequest(new { mensagem = "Aluno não encontrado" });
        }

        // Verificar se disciplina existe
        var disciplina = _context.Disciplinas.Find(frequencia.DisciplinaId);
        if (disciplina == null)
        {
            return BadRequest(new { mensagem = "Disciplina não encontrada" });
        }

        _context.Frequencias.Add(frequencia);
        _context.SaveChanges();
        return Created("", frequencia);
    }

    [HttpPut("{id}")]
    public IActionResult Atualizar(int id, [FromBody] Frequencia frequencia)
    {
        var frequenciaExistente = _context.Frequencias.Find(id);
        if (frequenciaExistente == null)
        {
            return NotFound(new { mensagem = "Frequência não encontrada" });
        }

        frequenciaExistente.AlunoId = frequencia.AlunoId;
        frequenciaExistente.DisciplinaId = frequencia.DisciplinaId;
        frequenciaExistente.Data = frequencia.Data;
        frequenciaExistente.Presente = frequencia.Presente;

        _context.SaveChanges();
        return Ok(frequenciaExistente);
    }

    [HttpDelete("{id}")]
    public IActionResult Excluir(int id)
    {
        var frequencia = _context.Frequencias.Find(id);
        if (frequencia == null)
        {
            return NotFound(new { mensagem = "Frequência não encontrada" });
        }

        _context.Frequencias.Remove(frequencia);
        _context.SaveChanges();
        return Ok(new { mensagem = "Frequência excluída com sucesso" });
    }

    [HttpGet("aluno/{alunoId}")]
    public IActionResult ListarPorAluno(int alunoId)
    {
        var frequencias = _context.Frequencias
            .Include(f => f.Aluno)
            .Include(f => f.Disciplina)
            .Where(f => f.AlunoId == alunoId)
            .ToList();

        return Ok(frequencias);
    }

    [HttpGet("disciplina/{disciplinaId}")]
    public IActionResult ListarPorDisciplina(int disciplinaId)
    {
        var frequencias = _context.Frequencias
            .Include(f => f.Aluno)
            .Include(f => f.Disciplina)
            .Where(f => f.DisciplinaId == disciplinaId)
            .ToList();

        return Ok(frequencias);
    }

    [HttpGet("data/{data}")]
    public IActionResult ListarPorData(string data)
    {
        if (!DateTime.TryParse(data, out DateTime dataFrequencia))
        {
            return BadRequest(new { mensagem = "Data inválida" });
        }

        var frequencias = _context.Frequencias
            .Include(f => f.Aluno)
            .Include(f => f.Disciplina)
            .Where(f => f.Data.Date == dataFrequencia.Date)
            .ToList();

        return Ok(frequencias);
    }

    [HttpPost("registrar-lote")]
    public async Task<IActionResult> RegistrarLote([FromBody] List<Frequencia> frequencias)
    {
        try
        {
            Console.WriteLine($"Registro em lote iniciado. Total de frequências: {frequencias?.Count ?? 0}");
            
            if (frequencias == null || !frequencias.Any())
            {
                Console.WriteLine("Erro: Lista de frequências é nula ou vazia");
                return BadRequest(new { mensagem = "Lista de frequências é obrigatória" });
            }

            // Validações básicas
            foreach (var frequencia in frequencias)
            {
                if (frequencia.AlunoId <= 0 || frequencia.DisciplinaId <= 0)
                {
                    Console.WriteLine($"Erro: AlunoId ou DisciplinaId inválido - AlunoId: {frequencia.AlunoId}, DisciplinaId: {frequencia.DisciplinaId}");
                    return BadRequest(new { mensagem = "Aluno e Disciplina são obrigatórios" });
                }
            }

            Console.WriteLine("Validações básicas passaram. Verificando existência de alunos e disciplinas...");

            // Verificar se todos os alunos existem
            var alunoIds = frequencias.Select(f => f.AlunoId).Distinct().ToList();
            var alunosExistentes = await _context.Alunos.Where(a => alunoIds.Contains(a.Id)).ToListAsync();
            
            if (alunosExistentes.Count != alunoIds.Count)
            {
                var alunosNaoEncontrados = alunoIds.Except(alunosExistentes.Select(a => a.Id)).ToList();
                Console.WriteLine($"Erro: Alunos não encontrados: {string.Join(", ", alunosNaoEncontrados)}");
                return BadRequest(new { mensagem = $"Alunos não encontrados: {string.Join(", ", alunosNaoEncontrados)}" });
            }

            // Verificar se todas as disciplinas existem
            var disciplinaIds = frequencias.Select(f => f.DisciplinaId).Distinct().ToList();
            var disciplinasExistentes = await _context.Disciplinas.Where(d => disciplinaIds.Contains(d.Id)).ToListAsync();
            
            if (disciplinasExistentes.Count != disciplinaIds.Count)
            {
                var disciplinasNaoEncontradas = disciplinaIds.Except(disciplinasExistentes.Select(d => d.Id)).ToList();
                Console.WriteLine($"Erro: Disciplinas não encontradas: {string.Join(", ", disciplinasNaoEncontradas)}");
                return BadRequest(new { mensagem = $"Disciplinas não encontradas: {string.Join(", ", disciplinasNaoEncontradas)}" });
            }

            Console.WriteLine("Alunos e disciplinas verificados. Verificando duplicatas...");

            // Verificar duplicatas (mesmo aluno, disciplina e data)
            var duplicatas = new List<string>();
            foreach (var frequencia in frequencias)
            {
                var existe = await _context.Frequencias
                    .AnyAsync(f => f.AlunoId == frequencia.AlunoId && 
                                  f.DisciplinaId == frequencia.DisciplinaId && 
                                  f.Data.Date == frequencia.Data.Date);
                
                if (existe)
                {
                    var aluno = alunosExistentes.First(a => a.Id == frequencia.AlunoId);
                    var disciplina = disciplinasExistentes.First(d => d.Id == frequencia.DisciplinaId);
                    duplicatas.Add($"{aluno.Nome} - {disciplina.Nome} - {frequencia.Data:dd/MM/yyyy}");
                }
            }

            if (duplicatas.Any())
            {
                Console.WriteLine($"Erro: Frequências duplicadas encontradas: {string.Join(", ", duplicatas)}");
                return BadRequest(new { 
                    mensagem = "Já existem registros de frequência para alguns alunos nesta data",
                    duplicatas = duplicatas
                });
            }

            Console.WriteLine("Nenhuma duplicata encontrada. Salvando frequências...");

            // Adicionar frequências ao contexto
            _context.Frequencias.AddRange(frequencias);
            
            // Salvar no banco
            var registrosSalvos = await _context.SaveChangesAsync();
            
            Console.WriteLine($"✅ Frequências registradas com sucesso! Registros salvos: {registrosSalvos}");
            Console.WriteLine($"📊 Resumo: {frequencias.Count} frequências, {frequencias.Count(f => f.Presente)} presentes, {frequencias.Count(f => !f.Presente)} ausentes");

            return Ok(new { 
                mensagem = "Frequências registradas com sucesso",
                totalRegistros = frequencias.Count,
                presentes = frequencias.Count(f => f.Presente),
                ausentes = frequencias.Count(f => !f.Presente),
                registrosSalvos = registrosSalvos
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao registrar frequências em lote: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            return StatusCode(500, new { mensagem = "Erro interno do servidor ao registrar frequências" });
        }
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

        // Verifica se já existe registro de frequência para este aluno nesta data
        var frequenciaExistente = _context.Frequencias
            .FirstOrDefault(f => f.AlunoId == frequencia.AlunoId && f.DisciplinaId == frequencia.DisciplinaId && f.Data.Date == frequencia.Data.Date);

        if (frequenciaExistente != null)
        {
            return BadRequest(new { mensagem = "Já existe registro de frequência para este aluno nesta data" });
        }

        _context.Frequencias.Add(frequencia);
        _context.SaveChanges();
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

        var faltas = _context.Frequencias
            .Where(f => f.AlunoId == request.AlunoId && f.Data.Date == request.Data.Date)
            .ToList();
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

        var faltas = _context.Frequencias
            .Where(f => f.AlunoId == request.AlunoId && f.DisciplinaId == request.DisciplinaId && f.Data.Date == request.Data.Date)
            .ToList();
        return Ok(new { totalFaltas = faltas.Count });
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

        var faltas = _context.Frequencias
            .Where(f => f.AlunoId == request.AlunoId && f.DisciplinaId == request.DisciplinaId && f.Data.Date == request.Data.Date)
            .ToList();
        return Ok(new {
            alunoId = request.AlunoId,
            disciplinaId = request.DisciplinaId,
            totalFaltas = faltas.Count
        });
    }
}

public class ConsultaFaltasRequest
{
    public int AlunoId { get; set; }
    public int DisciplinaId { get; set; }
    public DateTime Data { get; set; }
}