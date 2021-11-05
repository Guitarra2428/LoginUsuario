using AutoMapper;
using LoginUsuario.Models;
using LoginUsuario.Models.Dto;
using LoginUsuario.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LoginUsuario.Controllers
{
    [Authorize]
    [Route("api/Usuarios")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _repository;
        private readonly IMapper _imapper;
        private readonly IConfiguration _configuration;

        public UsuariosController(IUsuarioRepository repository, IMapper imapper, IConfiguration configuration)
        {
            _repository = repository;
            _imapper = imapper;
            _configuration = configuration;
        }
        /// <summary>
        /// Obtener Usuarios
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType (200, Type =typeof(List<UsuarioDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetUsuarios()
        {
            var datos = _repository.GetUsuarios();

            var datosDto = new List<UsuarioDto>();
            foreach (var lista in datos)
            {
                datosDto.Add(_imapper.Map<UsuarioDto>(lista));
            }

            return Ok(datosDto);
        }
        /// <summary>
        /// Obtener Usuario Individual por Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}", Name = "GetUsuario")]
        [ProducesResponseType(200, Type = typeof(UsuarioDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetUsuario(int id)
        {
            var datos = _repository.GetUsuario(id);

            if (datos==null)
            {
                return NotFound();
            }
           
             var datosDto=_imapper.Map<UsuarioDto>(datos);            

            return Ok(datosDto);
        }
        /// <summary>
        /// Registro de usuario
        /// </summary>
        /// <param name="usuarioRegistronDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Registro")]
        public IActionResult Registro(UsuarioRegistronDto usuarioRegistronDto)
        {
            usuarioRegistronDto.Usuario = usuarioRegistronDto.Usuario.ToLower();

            if (_repository.ExiteUsuario(usuarioRegistronDto.Usuario))
            {
                return BadRequest("El Usuario Ya Existe");
            }
           var usauACrear= new Usuario
            {
                UsuaroA = usuarioRegistronDto.Usuario
            };


            var useCreado = _repository.Registro(usauACrear, usuarioRegistronDto.Password);

            return Ok(useCreado);
        }
        /// <summary>
        /// Iniciar sesion con usuario
        /// </summary>
        /// <param name="usuarioLoginDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UsuarioLoginDto usuarioLoginDto)
        {
            var usuarioLoginDb = _repository.Login(usuarioLoginDto.Usuario, usuarioLoginDto.Password);
            if (usuarioLoginDb==null)
            {
                return Unauthorized();
            }

            var clain = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,usuarioLoginDb.Id.ToString()),
                new Claim(ClaimTypes.Name,usuarioLoginDb.UsuaroA.ToString())
            };


            //generar token

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);


            var tokendescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(clain),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credenciales
            };


            var tokenHndler = new JwtSecurityTokenHandler();
            var token = tokenHndler.CreateToken(tokendescriptor);

            return Ok(new
            {
                token = tokenHndler.WriteToken(token)
            });

        }
    }
}
