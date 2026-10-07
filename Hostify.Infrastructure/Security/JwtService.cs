using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Hostify.Aplication.Interfaces.Security;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Hostify.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Hostify.Infrastructure.Security
{
    public class JwtService : IJwtSevice
    {
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            // crea una clave de seguridad a partir de la clave secreta definida en la configuración
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            // crea las credenciales de firma utilizando la clave de seguridad y el algoritmo HMAC SHA256
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // crea una lista de claims que se incluirán en el token, como el identificador del usuario y su correo electrónico
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // agrega los roles del usuario como claims adicionales en el token
            var roles = user.Roles?
                .Select(r => r.Role.ToString())
                .ToList() ?? new List<string>();

            if (user.Roles != null && user.Roles.Any())
            {
                // itera sobre los roles del usuario y agrega un claim de tipo "Role" para cada uno
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            // crea un token JWT utilizando los claims, la fecha de expiración y las credenciales de firma
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
