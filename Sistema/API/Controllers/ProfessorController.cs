using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Configuration;
using API.Data;
using API.Repositories;
using API.Models;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

namespace API.Controllers;

[ApiController]
[Route("api/professor")]
public class ProfessorController : ControllerBase
{
    private readonly IProfessorRepository _professorRepository;
    private readonly JwtSettings _jwtSettings;
    
    public ProfessorController(IProfessorRepository professorRepository,
        IOptions<JwtSettings> jwtSettings)
    {
        _professorRepository = professorRepository;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpPost("cadastrar")]
    public IActionResult Cadastrar([FromBody] CadastrarProfessorDTO dto)
    {
        var professor = new Professor
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Senha = dto.Senha,
            Especialidade = dto.Especialidade
        };
        
        _professorRepository.Cadastrar(professor);
        return Created("", professor);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginProfessorDTO dto)
    {
        Professor? professorExistente = _professorRepository
            .BuscarProfessorPorEmailSenha(dto.Email, dto.Senha);

        if (professorExistente == null)
        {
            return Unauthorized(new { mensagem = "Professor ou senha inválidos!" });
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
            new Claim(ClaimTypes.Role, professor.Role),
            new Claim("nome", professor.Nome),
            new Claim("email", professor.Email)
        };

        var chave = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        var assinatura = new SigningCredentials(
            new SymmetricSecurityKey(chave),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(_jwtSettings.ExpirationHours),
            signingCredentials: assinatura
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}