using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Hostify.Aplication.Features.Reservations.Interfaces;
using Hostify.Aplication.Features.Reservations.DTOs;

namespace Hostify.API.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    [Authorize]
    public class ReservatiosController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservatiosController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        // POST: api/reservations/create

        [HttpPost("create")]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = Guid.Parse(userIdClaim);

            var result = await _reservationService.CreateAsync(userId, dto);

            return Ok(result);
        }

        // GET: api/reservations/my

        [HttpGet("my")]
        public async Task<IActionResult> GetMyReservations()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = Guid.Parse(userIdClaim);

            var result = await _reservationService.GetMyReservationsAsync(userId);

            return Ok(result);
        }

        // GET: api/reservations/host

        [HttpGet("host")]
        [Authorize(Roles = "Host")]
        public async Task<IActionResult> GetHostReservations()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            var userId = Guid.Parse(userIdClaim);

            var result = await _reservationService.GetHostReservationsAsync(userId);

            return Ok(result);
        }

        // POST: api/reservations/confirm/{reservationId}
        [HttpPost("confirm/{reservationId}")]
        [Authorize(Roles = "Host")]
        public async Task<IActionResult> ConfirmReservation(Guid reservationId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            var userId = Guid.Parse(userIdClaim);

            await _reservationService.ConfirmReservationAsync(reservationId, userId);

            return Ok(new { message = "Reserva confirmada" });
        }

        // POST: api/reservations/cancel/{reservationId}
        [HttpPost("cancel/{reservationId}")]
        [Authorize(Roles = "Host")]
        public async Task<IActionResult> CancelReservation(Guid reservationId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = Guid.Parse(userIdClaim);

            await _reservationService.CancelReservationAsync(reservationId, userId);

            return Ok(new { message = "Reserva cancelada" });
        }

        // POST: api/reservations/complete-expired
        [HttpPost("complete-expired")]
        [Authorize(Roles = "Host")]
        public async Task<IActionResult> CompleteExpiredReservations()
        {
            await _reservationService.CompleteExpiredReservationAsync();
            return Ok(new { message = "Reservas expiradas completadas" });
        }


    }
}