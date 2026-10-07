using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Aplication.Features.Reservations.DTOs;
using Hostify.Domain.Entities;

namespace Hostify.Aplication.Features.Reservations.Interfaces
{
    public interface IReservationService
    {
        Task<ReservationResponseDto> CreateAsync(Guid guestId, CreateReservationDto dto);

        Task<List<ReservationResponseDto>> GetMyReservationsAsync(Guid userId);

        Task<List<ReservationResponseDto>> GetHostReservationsAsync(Guid hostId);

        Task ConfirmReservationAsync(Guid reservationId, Guid hostId);

        Task CancelReservationAsync(Guid reservationId, Guid userId);
        Task CompleteExpiredReservationAsync();

        Task<List<ReservationResponseDto>> GetAllAsync();
    }
}
