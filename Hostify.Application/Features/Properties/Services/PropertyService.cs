using Hostify.Application.Features.Properties.DTOs;
using Hostify.Domain.Entities;
using Hostify.Application.Features.Properties.Interfaces;
using Hostify.Application.Features.Reviews.Interfaces;
using Hostify.Application.Features.Reservations.Interfaces;

namespace Hostify.Application.Features.Properties.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IReviewRepository _reviewRepository;   


        public PropertyService(IPropertyRepository propertyRepository, IReservationRepository reservationRepository, IReviewRepository reviewRepository)
        {
            _propertyRepository = propertyRepository;
            _reservationRepository = reservationRepository;
            _reviewRepository = reviewRepository;
        }

        public async Task<Property> CreateAsync(Guid hostId, CreatePropertyDto createPropertyDto)
        {
            var property = new Property
            {
                Id = Guid.NewGuid(),
                HostId = hostId,
                Title = createPropertyDto.Title,
                Description = createPropertyDto.Description,
                Location = createPropertyDto.Location,
                PricePerNight = createPropertyDto.PricePerNight,
                Capacity = createPropertyDto.Capacity,
                IsDeleted = false
            };

            await _propertyRepository.AddAsync(property);
            await _propertyRepository.SaveChangesAsync();

            return property;
        }

        // Esto es para obtener las propiedades de un host, no es para obtener una propiedad por su id
        public async Task<List<PropertyResponseDto>?> GetByHostPropertiesAsync(Guid hostId)
        {
            var properties = await _propertyRepository.GetByHostIdAsync(hostId);
            return properties?.Select(p => new PropertyResponseDto
            {
                Id = p.Id,
                HostId = p.HostId,
                Title = p.Title,
                Description = p.Description,
                Location = p.Location,
                PricePerNight = p.PricePerNight,
                Capacity = p.Capacity
            }).ToList();
        }
        // Esto es para obtener una propiedad por su id, no es para obtener las propiedades de un host
        public async Task UpdatePropertyAsync(Guid propertyId, Guid hostId, UpdatePropertyDto dto)
        {

            var property = await _propertyRepository.GetByIdAsync(propertyId);
            if (property == null)
                throw new Exception("Propiedad no encontrada");

            if (property.HostId != hostId)
                throw new Exception("No tienes permiso para actualizar esta propiedad");

            property.Title = dto.Title;
            property.PricePerNight = dto.PricePerNight;
            property.Location = dto.Location;
            property.Description = dto.Description;
            property.Capacity = dto.Capacity;


            await _propertyRepository.UpdateAsync(property);
        }

        // Esto es para eliminar una propiedad por su id, no es para eliminar las propiedades de un host
        public async Task SoftDeleteAsync(Guid propertyId, Guid hostId)
        {
            var property = await _propertyRepository.GetByIdAsync(propertyId);

            if (property == null)
                throw new Exception("Propiedad no encontrada");

            if (property.HostId != hostId)
                throw new Exception("No tienes permiso para eliminar esta propiedad");

            await _propertyRepository.SoftDeleteAsync(property);
        }

        // Esto es para obtener una propiedad por su id, no es para obtener las propiedades de un host
        public async Task<PropertyDetailDto> GetPropertyDetailAsync(Guid propertyId)
        {
            var property = await _propertyRepository.GetByIdAsync(propertyId);

            if (property == null)
                throw new Exception("Propiedad no encontrada");

            var reservationCount = await _propertyRepository.GetReservationCountAsync(propertyId);

            var isAvailable = !await _reservationRepository.ExistsOverlap(
                propertyId,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(1)
            );

            var reviews = await _reviewRepository.GetByPropertyIdAsync(propertyId);
            var average = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

            return new PropertyDetailDto
            {
                Id = property.Id,
                Title = property.Title,
                PricePerNight = property.PricePerNight,
                Location = property.Location,
                Description = property.Description,
                Capacity = property.Capacity,
                TotalReservations = reservationCount,
                IsAvailable = isAvailable,
                AverageRating = average
            };
        }

        public async Task<List<PropertyResponseDto>> SearchPropertiesAsync(PropertyFilterDto filter)
        {
            if (filter.CheckIn.HasValue && filter.CheckOut.HasValue)
            {
                if (filter.CheckIn >= filter.CheckOut)
                    throw new Exception("La fecha de check-in debe ser anterior a la fecha de check-out");
            }

            filter.Location ??= string.Empty;
            filter.MinPrice ??= 0;
            filter.MaxPrice ??= 99999999;
            filter.MinCapacity ??= 1;
            filter.MaxCapacity ??= int.MaxValue;
            filter.CheckIn ??= DateTime.UtcNow;
            filter.CheckOut ??= DateTime.UtcNow.AddYears(1);

            var properties = await _propertyRepository.SearchAsync(filter);

            return properties.Select(p => new PropertyResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Location = p.Location,
                PricePerNight = p.PricePerNight,
                Capacity = p.Capacity,
                HostId = p.HostId
            }).ToList();
        }
    }
}
