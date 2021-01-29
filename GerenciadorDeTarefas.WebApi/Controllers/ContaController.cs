﻿using AutoMapper;

using GerenciadorDeTarefas.Common.Models.Usuarios;
using GerenciadorDeTarefas.Domain.Contexto;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorDeTarefas.WebApi.Controllers
{
    [Route("api/conta")]
    [ApiController]
    public class ContaController : ControllerBase
    {
        private readonly IContextoDeDados _contexto;
        private readonly IMapper _mapper;

        public ContaController(IContextoDeDados contexto, IMapper mapper)
        {
            _contexto = contexto;
            _mapper = mapper;
        }

        [HttpPost, Route("logar"), AllowAnonymous]
        public async Task<ActionResult> Logar(string login, string senha)
        {
            senha = EncriptarSenha(login, senha);

            Domain.Usuarios.Usuario user = _contexto.Usuarios.FirstOrDefault(u => u.Login == login && u.Senha == senha);

            if (user == null)
                return NotFound();

            UsuarioModel usuarioModel = _mapper.Map<UsuarioModel>(user);
            usuarioModel.Token = TokenService.GenerateToken(usuarioModel);

            await Task.CompletedTask;

            return Ok(usuarioModel);
        }

        private string EncriptarSenha(string login, string senha)
        {
            byte[] salt = Encoding.UTF8.GetBytes(login);
            byte[] senhaByte = Encoding.UTF8.GetBytes(senha);
            byte[] sha256 = new SHA256Managed().ComputeHash(senhaByte.Concat(salt).ToArray());
            return Convert.ToBase64String(sha256);
        }
    }
}
