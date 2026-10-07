using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Application.Features.Properties.DTOs;
using Hostify.Domain.Entities;
using Hostify.Domain.Enums;
using Hostify.Application.Features.Properties.DTOs;

namespace Hostify.Application.Features.Properties.Interfaces
{
    public interface IPropertyService
    {
        Task<Property> CreateAsync(Guid hostId, CreatePropertyDto createPropertyDto);
        Task<List<PropertyResponseDto>> GetByHostPropertiesAsync(Guid hostId);
        Task UpdatePropertyAsync(Guid propertyId, Guid hostId, UpdatePropertyDto dto);
        Task SoftDeleteAsync(Guid propertyId, Guid hostId);
        Task<PropertyDetailDto> GetPropertyDetailAsync(Guid propertyId);
        Task<List<PropertyResponseDto>> SearchPropertiesAsync(PropertyFilterDto filter);
    }
}
