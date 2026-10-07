using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hostify.Aplication.Features.Reviews.Interfaces;
using Hostify.Aplication.Features.Reviews.DTOs;
using System.Security.Claims;
using Hostify.Aplication.Features.Auth.DTOs;

namespace Hostify.API.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // POST: api/reviews
        // Crea una nueva reseña para una propiedad específica. El cuerpo de la solicitud debe contener los detalles de la reseña, incluyendo el ID de la propiedad, el ID del huésped, la calificación y el comentario.
        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto review)
        {
            var userId =  User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (review.Rating < 1 || review.Rating > 5)
                return BadRequest("La calificación debe estar entre 1 y 5");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Usuario no autenticado");

            if (!Guid.TryParse(userId, out var guestId))
                return BadRequest("ID de usuario no válido");

            await _reviewService.CreateReviewAsync(guestId, review);

            return Ok("Reseña creada correctamente");
        }


        // GET: api/reviews/property/{propertyId}
        // Obtiene todas las reseñas asociadas a una propiedad específica, identificada por su ID. El resultado incluirá detalles de cada reseña, como la calificación, el comentario y la información del huésped que realizó la reseña.
        [HttpGet("property/{propertyId}")]
        public async Task<IActionResult> GetReviewsByPropertyId(Guid propertyId)
        {
            var result = await _reviewService.GetReviewsByPropertyIdAsync(propertyId);
            return Ok(result);
        }

        // PUT: api/reviews/{reviewId}
        // Actualiza una reseña existente identificada por su ID. El cuerpo de la solicitud debe contener los nuevos detalles de la reseña, como la calificación y el comentario. Solo el huésped que creó la reseña puede actualizarla.
        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateReview(Guid reviewId, [FromBody] UpdateReviewDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var parseUserId))
                return Unauthorized("Usuario no autenticado");

            await _reviewService.UpdateReviewAsync(reviewId, parseUserId, dto);

            return Ok("Reseña actualizada correctamente");
        }
    }
}
