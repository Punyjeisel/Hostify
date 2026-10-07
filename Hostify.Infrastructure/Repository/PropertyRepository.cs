using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Application.Features.Auth.DTOs;
using Hostify.Domain.Entities;
using Hostify.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Hostify.Application.Features.Properties.Interfaces;
using Hostify.Application.Features.Properties.DTOs;
using Hostify.Domain.Enums;

namespace Hostify.Infrastructure.Repository
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly HostifyDbContext _context;

        public PropertyRepository(HostifyDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Property property)
        {
            await _context.properties.AddAsync(property);
        }

        public async Task<Property?> GetByIdAsync(Guid id)
        {
            return await _context.properties
                .Include(p => p.BlockedDates)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<IEnumerable<Property>?> GetByHostIdAsync(Guid hostId)
        {
            return await _context.properties
                .Where(p => p.HostId == hostId && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetAllAsync()
        {
            return await _context.properties
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Property property)
        {
            _context.properties.Update(property);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Property property)
        {
            property.IsDeleted = true;
            _context.properties.Update(property);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetReservationCountAsync(Guid propertyId)
        {
            return await _context.reservations
                .CountAsync(r => r.PropertyId == propertyId);
        }

        public async Task<List<Property>> SearchAsync(PropertyFilterDto filter)
        {
            var query = _context.properties.AsQueryable();

            query = _context.properties.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Location))
                query = query.Where(p => p.Location.Contains(filter.Location));

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.PricePerNight >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.PricePerNight <= filter.MaxPrice.Value);

            if (filter.MinCapacity.HasValue)
                query = query.Where(p => p.Capacity >= filter.MinCapacity.Value);

            if (filter.MaxCapacity.HasValue)
                query = query.Where(p => p.Capacity <= filter.MaxCapacity.Value);


            // Revisa si las fechas de check-in y check-out están disponibles para la propiedad
            if (filter.CheckIn.HasValue && filter.CheckOut.HasValue)
            {
                query = query.Where(p => 
                !_context.reservations.Any(r =>
                    r.PropertyId == p.Id &&
                    r.Status != ReservationStatus.Cancelled && 
                    filter.CheckIn.Value < r.CheckOut &&
                    filter.CheckOut.Value > r.CheckIn
                ));
            }

            return await query.ToListAsync();
        }
    }
}
