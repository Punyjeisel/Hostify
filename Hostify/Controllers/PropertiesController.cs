using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hostify.Aplication.Features.Properties.DTOs;
using System.Security.Claims;
using Hostify.Aplication.Features.Properties.Interfaces;

namespace Hostify.API.Controllers
{
    [ApiController]
    [Route("api/properties")]
    [Authorize]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertiesController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        // POST: api/properties
        // Crea una nueva propiedad. Solo el host puede acceder a esta ruta.
        [Authorize(Policy = "HostOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateProperty([FromBody] CreatePropertyDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var hostId = Guid.Parse(userIdClaim);

            var property = await _propertyService.CreateAsync(hostId, dto);

            return Ok(property);
        }

        // GET: api/properties/my-properties
        // Obtiene todas las propiedades del host autenticado. Solo el host puede acceder a esta ruta.
        [HttpGet("my-properties")]
        [Authorize(Policy = "HostOnly")]
        public async Task<IActionResult> GetMyProperties()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _propertyService.GetByHostPropertiesAsync(userId);

            return Ok(result);
        }

        // PUT: api/properties/{Id}
        // Actualiza una propiedad específica. Solo el host que creó la propiedad puede actualizarla.
        [HttpPut("{Id}")]
        [Authorize(Policy = "HostOnly")]
        public async Task<IActionResult> UpdateProperty(Guid Id, [FromBody] UpdatePropertyDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _propertyService.UpdatePropertyAsync(Id, userId, dto);

            return Ok(new { message = "Propiedad actualizada correctamente" });
        }

        // DELETE: api/properties/{Id}
        // Elimina una propiedad específica. Solo el host que creó la propiedad puede eliminarla.
        [HttpDelete("{Id}")]
        [Authorize(Policy = "HostOnly")]
        public async Task<IActionResult> DeleteProperty(Guid Id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _propertyService.SoftDeleteAsync(Id, userId);

            return Ok(new { message = "Propiedad eliminada correctamente" });
        }

        // GET: api/properties/{id}
        // Obtiene los detalles de una propiedad específica. Cualquier usuario autenticado puede acceder a esta ruta.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPropertyById(Guid id)
        {
            var result = await _propertyService.GetPropertyDetailAsync(id);
            return Ok(result);
        }

        // GET: api/properties/search
        // Busca propiedades según los filtros proporcionados. Cualquier usuario autenticado puede acceder a esta ruta.
        [HttpGet("search")]
        public async Task<IActionResult> SearchProperties([FromQuery] PropertyFilterDto filter)
        {
            var result = await _propertyService.SearchPropertiesAsync(filter);
            return Ok(result);
        }
    }
}
