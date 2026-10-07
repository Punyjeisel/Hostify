using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Domain.Entities;

namespace Hostify.Aplication.Features.Reviews.Interfaces
{
    public interface IReviewRepository
    {
        Task AddAsync (Review review);
        Task<List<Review>> GetByPropertyIdAsync(Guid propertyId);
        Task<bool> HasCompletedReservation(Guid userId, Guid propertyId);
        Task<bool>ExistsByReservationIdAsync(Guid reservationId);
        Task<Review?> GetByIdAsync(Guid reviewId);
        Task UpdateAsync (Review review);
        Task<Review?> GetByReservationIdAsync(Guid reservationId);
    }
}
