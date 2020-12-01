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
        public async Task<ActionResult> Logar(UsuarioModel model)
        {
            model.Senha = EncriptarSenha(model);

            Domain.Usuarios.Usuario user = _contexto.Usuarios.FirstOrDefault(u => u.Login == model.Login && u.Senha == model.Senha);

            if (user == null)
                return NotFound();

            UsuarioModel usuarioModel = _mapper.Map<UsuarioModel>(user);
            usuarioModel.Token = TokenService.GenerateToken(usuarioModel);

            await Task.CompletedTask;

            return Ok(usuarioModel);
        }

        public string EncriptarSenha(UsuarioModel usuario)
        {
            byte[] salt = Encoding.UTF8.GetBytes(usuario.Login);
            byte[] senhaByte = Encoding.UTF8.GetBytes(usuario.Senha);
            byte[] sha256 = new SHA256Managed().ComputeHash(senhaByte.Concat(salt).ToArray());
            return Convert.ToBase64String(sha256);
        }
    }
}
