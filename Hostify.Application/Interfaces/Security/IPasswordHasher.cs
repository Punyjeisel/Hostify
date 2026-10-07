using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hostify.Application.Interfaces.Security
{
    public interface IPasswordHasher // Esta interfaz define un contrato para el hashing de contraseñas. Cualquier clase que implemente esta interfaz debe proporcionar una implementación del método Hash, que toma una contraseña como entrada y devuelve su versión hasheada. Esto permite que la aplicación utilice diferentes algoritmos de hashing de contraseñas sin depender de una implementación específica, lo que mejora la flexibilidad y la seguridad.
    {
        string Hash(string password);
        bool Verify(string password, string hashedPassword); // Este método se espera que tome una contraseña sin formato y una contraseña hasheada, y devuelva un valor booleano que indique si la contraseña sin formato coincide con la contraseña hasheada. Esto es útil para verificar las credenciales de los usuarios durante el proceso de autenticación.
    }
}
