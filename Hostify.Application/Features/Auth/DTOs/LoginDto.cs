using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hostify.Application.Features.Auth.DTOs
{
    public class LoginDto // Este DTO se utiliza para capturar la información necesaria para el inicio de sesión del usuario.
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
