﻿using AutoMapper;

using GerenciadorDeTarefas.Common.Models.Usuarios;
using GerenciadorDeTarefas.Domain.Contexto;
using GerenciadorDeTarefas.Domain.Usuarios;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GerenciadorDeTarefas.WebApi.Controllers
{
    [Route("api/usuario")]
    public class UsuarioController : BaseApiController
    {
        private readonly IContextoDeDados _contexto;
        private readonly IMapper _mapper;

        public UsuarioController(IContextoDeDados contexto, IMapper mapper)
        {
            _contexto = contexto;
            _mapper = mapper;
        }

        [HttpGet, Route("{id}")]
        public async Task<ActionResult> BuscarUsuarioPorId(int id)
        {
            var usuario = _contexto.Usuarios.Find(id);

            await Task.CompletedTask;

            return Ok(_mapper.Map<UsuarioModel>(usuario));
        }

        [HttpGet]
        public async Task<ActionResult> BuscarUsuarioPorEmail (string email)
        {
            var usuario = _contexto.Usuarios.FirstOrDefault(u => u.Pessoa.Email == email);

            if (usuario == null)
                return NotFound();

            await Task.CompletedTask;

            return Ok(_mapper.Map<UsuarioModel>(usuario));
        }

        [HttpGet, Route("verifica/email")]
        public Task<bool> VerificaEmail(string email)
        {
            return Task.FromResult(_contexto.Pessoas.Any(p => p.Email == email));
        }        
        
        [HttpGet, Route("verifica/login")]
        public Task<bool> VerificaLogin(string login)
        {
            return Task.FromResult(_contexto.Usuarios.Any(u => u.Login == login));
        }

        [HttpPut]
        public async Task<ActionResult> AtualizarUsuario ([FromBody] UsuarioModel usuarioModel)
        {
            Usuario usuario = _mapper.Map<Usuario>(usuarioModel);

            if (usuario.Login == null)
                throw new Exception("Login não deve ser nulo");

            if (usuario.Senha == null)
                throw new Exception("Senha não pode ser nula");

            Usuario existente = _contexto.Usuarios.FirstOrDefault(u => u.Login == usuario.Login);

            if(existente != null)
            {
                usuario.Senha = existente.Senha;

                _contexto.Usuarios.Update(usuario);
            }
            else
            {
                await _contexto.Usuarios.AddAsync(usuario);
            }

            await _contexto.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> NovoUsuario([FromBody] UsuarioModel usuarioModel)
        {
            if (usuarioModel.Login == null)
                return BadRequest("Login não deve ser nulo!");

            if (usuarioModel.Senha == null)
                return BadRequest("A senha não deve ser nula!");

            if (usuarioModel.Pessoa.Email == null)
                return BadRequest("Email não deve ser nulo!");

            if (_contexto.Usuarios.Any(u => u.Login == usuarioModel.Login))
                return BadRequest("Este login já está sendo utilizado.");

            if (_contexto.Pessoas.Any(p => p.Email == usuarioModel.Pessoa.Email))
                return BadRequest("Este email já está sendo utilizado.");

            Usuario usuario = _mapper.Map<Usuario>(usuarioModel);

            await _contexto.AddAsync(usuario);
            await _contexto.SaveChangesAsync();

            return Ok();
        }

    }
}
