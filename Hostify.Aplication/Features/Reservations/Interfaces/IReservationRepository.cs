using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Domain.Entities;

namespace Hostify.Aplication.Features.Reservations.Interfaces
{
    public interface IReservationRepository
    {
        Task AddAsync(Reservation reservation);
        Task UpdateAsync(Reservation reservation);
        Task<bool> ExistsOverlap(Guid propertyId, DateTime startDate, DateTime endDate); 
        Task<List<Reservation>> GetByGuestIdAsync(Guid guestId);
        Task<List<Reservation>> GetByHostReservationsAsync(Guid hostId);
        Task<Reservation?> GetByWithPropertyAsync(Guid Id);
        Task<List<Reservation>> GetAllAsync();
        Task<bool> TryCreateReservationAsync(Reservation reservation);
        Task<bool> HasBlockedDates(Guid propertyId, DateTime checkIn, DateTime checkOut);
    }
}
