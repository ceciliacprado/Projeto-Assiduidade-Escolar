using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;

[ApiController]
[Route("api/aluno")]
public class AlunoController : ControllerBase
{
    private readonly IAlunoRepository _alunoRepository;
    private readonly IConfiguration _configuration;

    public AlunoController(IAlunoRepository alunoRepository, IConfiguration configuration)
    {
        _alunoRepository = alunoRepository;
        _configuration = configuration;
    }

    [HttpPost("cadastrar")]
    [Authorize(Roles = "administrador")]
    public IActionResult Cadastrar([FromBody] Aluno aluno)
    {
        _alunoRepository.Cadastrar(aluno);
        return Created("", aluno);
    }

    [HttpGet("listar")]
    [Authorize]
    public IActionResult Listar()
    {        
        return Ok(_alunoRepository.Listar());
    }

    // [ApiExplorerSettings(IgnoreApi = true)]
    // public string GerarToken(Aluno aluno)
    // {
    //     var claims = new[]
    //     {
    //         new Claim(ClaimTypes.Name, aluno.Email),
    //         new Claim(ClaimTypes.Role, aluno.Permissao.ToString())
    //     };

    //     var chave = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!);
        
    //     var assinatura = new SigningCredentials(
    //         new SymmetricSecurityKey(chave),
    //         SecurityAlgorithms.HmacSha256
    //     );

    //     var token = new JwtSecurityToken(
    //         claims: claims,
    //         expires: DateTime.Now.AddHours(1),
    //         signingCredentials: assinatura
    //     );
        
    //     return new JwtSecurityTokenHandler().WriteToken(token);
    // }

}