using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Application.Features.Properties.DTOs;
using Hostify.Domain.Entities;

namespace Hostify.Application.Features.Properties.Interfaces
{
    public interface IPropertyRepository
    {
        Task AddAsync(Property property);
        Task<Property?> GetByIdAsync(Guid Id);
        Task<IEnumerable<Property>> GetAllAsync();
        Task<IEnumerable<Property>?> GetByHostIdAsync(Guid hostId);
        Task SaveChangesAsync();
        Task UpdateAsync(Property property);
        Task SoftDeleteAsync (Property property);
        Task<int> GetReservationCountAsync(Guid propertyId);
        Task<List<Property>> SearchAsync(PropertyFilterDto filter);
    }
}
