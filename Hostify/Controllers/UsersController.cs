using System.Security.Claims;
using Hostify.Aplication.Interfaces.Security;
using Hostify.Domain.Entities;
using Hostify.Domain.Enums;
using Hostify.Infrastructure.Persistence;
using Hostify.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hostify.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly HostifyDbContext _context;
        private readonly IJwtSevice _jwtService;

        public UsersController(HostifyDbContext context, IJwtSevice jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [Authorize]
        [HttpPost("become-host")]
        public async Task<IActionResult> BecomeHost()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim);

            var user = await _context.User
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                    return NotFound();

            var AlreadyHost = user.Roles.Any(r => r.Role == UserRole.Host); // Revisar si el usuario ya tiene el rol de Host

            if (AlreadyHost)
            {
                var token = _jwtService.GenerateToken(user); // Generar un nuevo token con el rol actualizado
                return Ok(new
                {
                    Message = "Ya eres un host.",
                    Token = token
                });
            } 
            
            user.Roles.Add(new UserRoleEntity
            {
                UserId = user.Id,
                Role = UserRole.Host
            });

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            var newToken = _jwtService.GenerateToken(user); // Generar un nuevo token con el rol actualizado

            return Ok(new
            { 
                Message = "Ahora eres un host.", 
                Token = newToken 
            });
        }
    }
}
