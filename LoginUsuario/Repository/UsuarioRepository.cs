using LoginUsuario.Data;
using LoginUsuario.Models;
using LoginUsuario.Repository.IRepository;
using System.Collections.Generic;
using System.Linq;

namespace LoginUsuario.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _db;

        public UsuarioRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool ExiteUsuario(string nombre)
        {
            if (_db.Usuario.Any(x=>x.UsuaroA==nombre))
            {
                return true;
            }
            return false;
        }

        public Usuario GetUsuario(int Idusario)
        {
            return _db.Usuario.FirstOrDefault(x=>x.Id==Idusario);
        }

        public ICollection<Usuario> GetUsuarios()
        {
            return _db.Usuario.OrderBy(x => x.UsuaroA).ToList();
        }

        public bool Guardar()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public Usuario Login(string usuario, string password)
        {
            var user = _db.Usuario.FirstOrDefault(x=>x.UsuaroA==usuario);

            if (user==null)
            {
                return null;

            }
            if (!VerificacionPassword(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return user;
        }

        public Usuario Registro(Usuario usuario, string password)
        {

            byte[] passwordHash, passwordSalt;

            CrearPassworHash(password, out passwordHash, out passwordSalt);

            usuario.PasswordHash = passwordHash;
            usuario.PasswordSalt = passwordSalt;
            _db.Add(usuario);
            Guardar();
            return usuario;
        }

        private bool VerificacionPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac= new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var hashComputado = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < hashComputado.Length; i++)
                {
                    if (hashComputado[i] !=passwordHash[i])
                    {
                        return false;
                    }
                }
                return true;

            }
        }


        private void CrearPassworHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
           

            }
        }


    }

}
    

