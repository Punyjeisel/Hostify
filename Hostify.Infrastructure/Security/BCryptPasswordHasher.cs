using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Application.Interfaces.Security;


namespace Hostify.Infrastructure.Security
{
    public class BCryptPasswordHasher : IPasswordHasher // Esta clase implementa la interfaz IPasswordHasher y utiliza la biblioteca BCrypt para realizar el hashing de contraseñas. El método Hash toma una contraseña como entrada y devuelve su versión hasheada utilizando BCrypt.
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // El método Verify toma una contraseña sin formato y una contraseña hasheada, y devuelve un valor booleano que indica si la contraseña sin formato coincide con la contraseña hasheada utilizando BCrypt.
        public bool Verify(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
