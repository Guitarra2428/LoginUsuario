using AutoMapper;
using LoginUsuario.Models;
using LoginUsuario.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginUsuario.UsuarioMapper
{
    public class UsauarioMappers: Profile
    {
        public UsauarioMappers()
        {
            CreateMap<Usuario, UsuarioDto>().ReverseMap();
            CreateMap<Usuario, UsuarioRegistronDto>().ReverseMap();
            CreateMap<Usuario, UsuarioLoginDto>().ReverseMap();

        }

    }
}
