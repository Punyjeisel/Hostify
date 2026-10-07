using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hostify.Application.Features.Auth.DTOs
{
    public class RegisterDto// este DTO se utiliza para capturar la información necesaria para el registro de un nuevo usuario en el sistema.
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
