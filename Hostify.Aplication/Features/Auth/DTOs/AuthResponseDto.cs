using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hostify.Aplication.Features.Auth.DTOs
{
    public class AuthResponseDto // Este DTO se utiliza para encapsular la respuesta que se envía al cliente después de un proceso de autenticación exitoso, como el inicio de sesión o el registro. Contiene el token de autenticación generado y el correo electrónico del usuario autenticado.
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
    }
}
