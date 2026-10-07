using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Aplication.Features.Reviews.DTOs;

namespace Hostify.Aplication.Features.Reviews.Interfaces
{
    public interface IReviewService
    {
        Task CreateReviewAsync(Guid guestId, CreateReviewDto dto);
        Task<List<ReviewResponseDto>> GetReviewsByPropertyIdAsync(Guid propertyId);
        Task UpdateReviewAsync (Guid reviewId, Guid userId, UpdateReviewDto dto);
    }
}
