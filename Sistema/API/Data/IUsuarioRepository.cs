using System;
using API.Models;

namespace API.Data;

public interface IUsuarioRepository
{
    void Cadastrar(Usuario usuario);
    List<Usuario> Listar();
    Usuario? BuscarUsuarioPorEmailSenha(string email, string senha);
}
