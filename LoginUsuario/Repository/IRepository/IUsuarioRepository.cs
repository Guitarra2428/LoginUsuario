using LoginUsuario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginUsuario.Repository.IRepository
{
    public  interface IUsuarioRepository
    {
        ICollection<Usuario> GetUsuarios();
        Usuario GetUsuario(int Idusario);
        bool  ExiteUsuario(string nombre);
        Usuario Registro(Usuario usuario, string password);
        Usuario Login(string usuario, string password);
        bool Guardar();

    }
}
