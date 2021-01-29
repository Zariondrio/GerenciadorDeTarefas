﻿using AutoMapper;

using GerenciadorDeTarefas.Common.Models.Equipes;
using GerenciadorDeTarefas.Common.Models.Projetos;
using GerenciadorDeTarefas.Common.Models.Usuarios;
using GerenciadorDeTarefas.Domain.Contexto;
using GerenciadorDeTarefas.Domain.Equipes;
using GerenciadorDeTarefas.Domain.ManyToMany;
using GerenciadorDeTarefas.Domain.Usuarios;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GerenciadorDeTarefas.WebApi.Controllers
{
    [Route("api/equipe")]
    public class EquipeController : BaseApiController
    {
        private readonly IContextoDeDados _contexto;
        private readonly IMapper _mapper;

        public EquipeController(IContextoDeDados contexto, IMapper mapper)
        {
            _contexto = contexto;
            _mapper = mapper;
        }

        [HttpGet, Route("{id}")]
        public async Task<EquipeModel> BuscarEquipe(int id)
        {
            Equipe equipe = _contexto.Equipes.Find(id);

            await Task.CompletedTask;

            return _mapper.Map<EquipeModel>(equipe);
        }

        [HttpGet, Route("usuarios/{idEquipe}")]
        public async Task<List<UsuarioModel>> UsuariosDaEquipe(int idEquipe)
        {
            List<Usuario> usuarios = _contexto.EquipeUsuario
                .Where(eu => eu.IdEquipe == idEquipe)
                .Select(e => e.Usuario)
                .ToList();

            List<UsuarioModel> models = new List<UsuarioModel>();

            foreach (var usuario in usuarios)
            {
                var model = _mapper.Map<UsuarioModel>(usuario);

                models.Add(model);
            }

            await Task.CompletedTask;
            return models;
        }

        [HttpGet, Route("projetos/{idEquipe}")]
        public async Task<List<ProjetoModel>> ProjetosDaEquipe(int idEquipe)
        {
            var projetos = _contexto.Projetos.Where(p => p.IdEquipe == idEquipe);

            List<ProjetoModel> models = new List<ProjetoModel>();

            foreach (var projeto in projetos)
            {
                var model = _mapper.Map<ProjetoModel>(projeto);
                models.Add(model);
            }

            return models;
        }

        [HttpPut, Route("adicionar/usuario/{idEquipe}")]
        public async Task<ActionResult> AdicionarUsuario([FromBody] UsuarioModel usuarioModel, int idEquipe)
        {
            Equipe equipe = _contexto.Equipes.Find(idEquipe);
            Usuario usuario = _contexto.Usuarios.FirstOrDefault(u => u.Login == usuarioModel.Login);

            EquipeUsuario equipeUsuario = new EquipeUsuario
            {
                Equipe = equipe,
                IdEquipe = equipe.Id,
                IdUsuario = usuario.IdPessoa,
                Usuario = usuario
            };

            equipe.Usuarios.Add(equipeUsuario);
            _contexto.SaveChanges();

            await Task.CompletedTask;

            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> AtualizarEquipe([FromBody] EquipeModel equipeModel)
        {
            Equipe equipe = _contexto.Equipes.FirstOrDefault(e => e.Id == equipeModel.Id);

            if (equipe == null)
            {
                return NotFound();
            }

            equipe.Nome = equipeModel.Nome;

            _contexto.Update(equipe);
            _contexto.SaveChanges();

            await Task.CompletedTask;

            return Ok();
        }

        [HttpPost, Route("{idUsuario}")]
        public async Task<ActionResult> CadastrarEquipe([FromBody] EquipeModel equipeModel, int idUsuario)
        {
            if (idUsuario <= 0)
                return NotFound("Usuario inválido");

            Equipe equipe = _mapper.Map<Equipe>(equipeModel);

            await equipe.Validate();

            Usuario usuario = _contexto.Usuarios.Find(idUsuario);

            _contexto.Add(equipe);
            _contexto.SaveChanges();

            EquipeUsuario equipeUsuario = new EquipeUsuario
            {
                Usuario = usuario,
                IdUsuario = usuario.IdPessoa,
                Equipe = equipe,
                IdEquipe = equipe.Id
            };

            usuario.Equipes.Add(equipeUsuario);
            equipe.Usuarios.Add(equipeUsuario);

            _contexto.Update(usuario);
            _contexto.Update(equipe);
            _contexto.SaveChanges();

            return Ok();
        }

        [HttpDelete, Route("{idEquipe}/{idUsuario}")]
        public async Task<ActionResult> ExcluirEquipe(int idEquipe, int idUsuario)
        {
            Equipe equipe = _contexto.Equipes.Find(idEquipe);
            EquipeUsuario usuario = equipe.Usuarios.FirstOrDefault(u => u.IdUsuario == idUsuario);

            if (equipe == null || usuario == null)
                return NotFound();

            if(usuario.)

            await Task.CompletedTask;
            return Ok();
        }


    }
}
