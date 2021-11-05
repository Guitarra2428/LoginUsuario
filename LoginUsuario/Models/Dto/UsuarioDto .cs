using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginUsuario.Models.Dto
{
    public class UsuarioDto
    {
      
        public string UsuaroA { get; set; }
        public byte[] PasswordHash { get; set; }
    }
}
