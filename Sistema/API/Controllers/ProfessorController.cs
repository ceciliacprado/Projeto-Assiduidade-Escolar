using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Data;
using API.Repositories;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;

[ApiController]
[Route("api/professor")]
public class ProfessorController : ControllerBase
{
    private readonly IProfessorRepository _professorRepository;
    private readonly IConfiguration _configuration;
    public ProfessorController(IProfessorRepository professorRepository,
        IConfiguration configuration)
    {
        _professorRepository = professorRepository;
        _configuration = configuration;
    }

    [HttpPost("cadastrar")]
    public IActionResult Cadastrar([FromBody] Professor professor)
    {
        if (professor.Role?.ToLower() != "admin")
        {
            return BadRequest(new { mensagem = "A role deve ser 'admin' para professores" });
        }
        _professorRepository.Cadastrar(professor);
        return Created("", professor);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] Professor professor)
    {
        Professor? professorExistente = _professorRepository
            .BuscarProfessorPorEmailSenha(professor.Email, professor.Senha);

        if (professorExistente == null)
        {
            return Unauthorized(new { mensagem = "Professor ou senha inválidos!" });
        }

        if (professorExistente.Role?.ToLower() != "admin")
        {
            return Unauthorized(new { mensagem = "Professor não tem permissão de administrador!" });
        }

        string token = GerarToken(professorExistente);
        return Ok(token);
    }

    [HttpGet("listar")]
    public IActionResult Listar()
    {
        return Ok(_professorRepository.Listar());
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public string GerarToken(Professor professor)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, professor.Email),
            new Claim(ClaimTypes.Role, "admin"),
            new Claim("nome", professor.Nome),
            new Claim("email", professor.Email)
        };

        var chave = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!);

        var assinatura = new SigningCredentials(
            new SymmetricSecurityKey(chave),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddSeconds(3000000),
            signingCredentials: assinatura
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}