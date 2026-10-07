using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Application.Features.Reservations.Interfaces;
using Hostify.Domain.Entities;
using Hostify.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Hostify.Domain.Enums;
using Hostify.Application.Features.Reservations.DTOs;
using Microsoft.EntityFrameworkCore.Storage;
using Hostify.Application.Features.Notifications.Interfaces;

namespace Hostify.Infrastructure.Repository
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly HostifyDbContext _context;
        private readonly INotificationService _notificationService;

        public ReservationRepository(HostifyDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }


        // Agrega una nueva reserva a la base de datos
        public async Task AddAsync(Reservation reservation)
        {
            await _context.reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
        }

        // Obtiene una reserva por su ID, incluyendo la propiedad y el huésped relacionados
        public async Task UpdateAsync(Reservation reservation)
        {
            _context.reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }

        // Verifica si existe alguna reserva confirmada para la propiedad que se superponga con las fechas de check-in y check-out proporcionadas
        public async Task<bool> ExistsOverlap(Guid propertyId, DateTime checkIn, DateTime checkOut)
        {
            // Verifica si existe alguna reserva confirmada para la propiedad que se superponga con las fechas de check-in y check-out proporcionadas
            return await _context.reservations.AnyAsync(r =>
                r.PropertyId == propertyId &&
                r.Status == ReservationStatus.Confirmed &&
                (
                    checkIn < r.CheckOut && checkOut > r.CheckIn // Revisa si la fecha de entrada o salida de la nueva reserva se superpone con las fechas de una reserva existente
                )
            );
        }

        // Obtiene todas las reservas asociadas a un huésped específico, incluyendo la propiedad relacionada
        public async Task<List<Reservation>> GetByGuestIdAsync(Guid guestId)
        {
            return await _context.reservations
                .Include(r => r.Property)
                .Where(r => r.GuestId == guestId)
                .ToListAsync();
        }

        // Obtiene todas las reservas asociadas a un anfitrión específico, incluyendo la propiedad relacionada
        public async Task<List<Reservation>> GetByHostReservationsAsync(Guid hostId)
        {
            return await _context.reservations
                .Include(r => r.Property)
                .Where(r => r.Property.HostId == hostId)
                .ToListAsync();
        }

        // Obtiene una reserva por su ID, incluyendo la propiedad y el huésped relacionados
        public async Task<Reservation?> GetByWithPropertyAsync(Guid id)
        {
            return await _context.reservations
                .Include(r => r.Property)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<Reservation>> GetAllAsync()
        {
            return await _context.reservations
                .Include(r => r.Property)
                .ToListAsync();
        }

        public async Task<bool> TryCreateReservationAsync(Reservation reservation)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var overlapExists = await _context.reservations
                .FromSqlRaw("SELECT * FROM reservations WITH (UPDLOCK, HOLDLOCK)")
                .AnyAsync(r =>
                    r.PropertyId == reservation.PropertyId &&
                    r.Status == ReservationStatus.Confirmed &&
                    reservation .CheckIn < r.CheckOut && 
                    reservation.CheckOut > r.CheckIn
                );

            if (overlapExists)
                return false;

            var blocked = await HasBlockedDates(
                reservation.PropertyId, 
                reservation.CheckIn, 
                reservation.CheckOut
            );

            if (blocked)
                return false;

            await _context.reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return true;
        }

        public async Task<bool> HasBlockedDates(Guid propertyId, DateTime checkIn, DateTime checkOut)
        {
            return await _context.blockedDates.AnyAsync(b =>
                b.PropertyId == propertyId &&
                b.StartDate >= checkIn &&
                b.EndDate <= checkOut
            );
        }
    }
}
