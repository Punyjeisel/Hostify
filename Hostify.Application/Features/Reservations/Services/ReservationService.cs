using Hostify.Application.Features.Reservations.Interfaces;
using Hostify.Application.Features.Properties.Interfaces;
using Hostify.Domain.Entities;
using Hostify.Application.Features.Reservations.DTOs;
using Hostify.Domain.Enums;
using Hostify.Application.Features.Notifications.Interfaces;




namespace Hostify.Application.Features.Reservations.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly INotificationService _notificationService;

        public ReservationService(IReservationRepository reservationRepository, IPropertyRepository propertyRepository, INotificationService notificationService)
        {
            _reservationRepository = reservationRepository;
            _propertyRepository = propertyRepository;
            _notificationService = notificationService;
        }

        // Crear una reserva
        public async Task<ReservationResponseDto> CreateAsync(Guid userId, CreateReservationDto dto)
        {
            // Buscar la propiedad
            var property = await _propertyRepository.GetByIdAsync(dto.PropertyId);

            if (property == null)
                throw new Exception("Propiedad no encontrada");

            // No se puede reservar tu propia propiedad
            if (property.HostId == userId)
                throw new Exception("No puedes reservar tu propia propiedad");

            // validar fechas
            if (dto.CheckIn >= dto.CheckOut)
                throw new Exception("La fecha de check-in debe ser anterior a la fecha de check-out");

            // Validad solapamiento con otras reservas
            var hasOverlap = await _reservationRepository.ExistsOverlap(
                dto.PropertyId,
                dto.CheckIn,
                dto.CheckOut
            );

            if (hasOverlap)
                throw new Exception("La propiedad ya está reservada para las fechas seleccionadas");

            // Calcular el precio total
            var nights = (dto.CheckOut.Date - dto.CheckIn.Date).Days;

            var totalPrice = nights * property.PricePerNight;

            // Crear la reserva
            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                PropertyId = property.Id,
                GuestId = userId,
                CheckIn = dto.CheckIn,
                CheckOut = dto.CheckOut,
                Status = ReservationStatus.Pending,
                TotalPrice = totalPrice
            };

            var created = await _reservationRepository.TryCreateReservationAsync(reservation);

            if (created)
            {
                await _notificationService.CreateAsync(reservation.GuestId, "Tu reserva fue creada"
                );

                await _notificationService.CreateAsync(property.HostId, "Nueva reserva recibida");
            }

            if (!created)
                throw new Exception("La propiedad ya no está disponible en esas fechas");

            await _notificationService.CreateAsync(property.HostId, "Nueva reserva recibida");
            await _notificationService.CreateAsync(userId, "Tu reserva fue creada");

            return new ReservationResponseDto
            {
                Id = reservation.Id,
                PropertyId = reservation.PropertyId,
                CheckIn = reservation.CheckIn,
                CheckOut = reservation.CheckOut,
                PropertyTitle = property.Title,
                TotalPrice = reservation.TotalPrice,
                Status = reservation.Status
            };
        }

        // Obtener reservas por usuario
        public async Task<List<ReservationResponseDto>> GetMyReservationsAsync(Guid userId)
        {
            var reservations = await _reservationRepository.GetByGuestIdAsync(userId);

            return reservations.Select(r => new ReservationResponseDto
            {
                Id = r.Id,
                PropertyId = r.PropertyId,
                CheckIn = r.CheckIn,
                CheckOut = r.CheckOut,
                PropertyTitle = r.Property.Title,
                TotalPrice = r.TotalPrice,
                Status = r.Status
            }).ToList();
        }

        // Obtener reservas por anfitrión
        public async Task<List<ReservationResponseDto>> GetHostReservationsAsync(Guid hostId)
        {
            var reservations = await _reservationRepository.GetByHostReservationsAsync(hostId);
            return reservations.Select(r => new ReservationResponseDto
            {
                Id = r.Id,
                PropertyId = r.PropertyId,
                CheckIn = r.CheckIn,
                CheckOut = r.CheckOut,
                PropertyTitle = r.Property.Title,
                TotalPrice = r.TotalPrice,
                Status = r.Status
            }).ToList();
        }

        // Confirmar una reserva
        public async Task ConfirmReservationAsync(Guid reservId, Guid hostId)
        {
            var reservation = await _reservationRepository.GetByWithPropertyAsync(reservId);
            if (reservation == null)
                throw new Exception("Reserva no encontrada");
            if (reservation.Property.HostId != hostId)
                throw new Exception("No tienes permiso para confirmar esta reserva");
            reservation.Status = ReservationStatus.Confirmed;
            await _reservationRepository.UpdateAsync(reservation);
        }

        // Cancelar una reserva (puede ser el huésped o el anfitrión)
        public async Task CancelReservationAsync(Guid reservationId, Guid userId)
        {
            var reservation = await _reservationRepository.GetByWithPropertyAsync(reservationId);

            if (reservation == null)
                throw new Exception("Reserva no encontrada");

            if (reservation.GuestId != userId && reservation.Property.HostId != userId)
                throw new Exception("No tienes permiso para cancelar esta reserva");

            reservation.Status = ReservationStatus.Cancelled;

            await _notificationService.CreateAsync(reservation.Property.HostId, "Reserva cancelada");
            await _notificationService.CreateAsync(reservation.GuestId, "Has cancelado la reserva");

            await _reservationRepository.UpdateAsync(reservation);
        }

        // Completar reservas expiradas (tarea programada)
        public async Task CompleteExpiredReservationAsync()
        {
            var reservations = await _reservationRepository.GetAllAsync();

            var now = DateTime.UtcNow;

            var toComplete = reservations
                .Where(r => r.Status == ReservationStatus.Confirmed && r.CheckOut < now)
                .ToList();

            foreach (var reservation in toComplete)
            {
                reservation.Status = ReservationStatus.Completed;
                await _notificationService.CreateAsync(reservation.Property.HostId, "Reserva completada");
                await _notificationService.CreateAsync(reservation.GuestId, "Tu reserva ha sido completada");

                await _reservationRepository.UpdateAsync(reservation);
            }


        }

        public async Task<List<ReservationResponseDto>> GetAllAsync()
        {
            var reservations = await _reservationRepository.GetAllAsync();

            return reservations.Select(r => new ReservationResponseDto
            {
                Id = r.Id,
                PropertyId = r.PropertyId,
                CheckIn = r.CheckIn,
                CheckOut = r.CheckOut,
                PropertyTitle = r.Property.Title,
                TotalPrice = r.TotalPrice,
                Status = r.Status
            }).ToList();
        }
    }
}
