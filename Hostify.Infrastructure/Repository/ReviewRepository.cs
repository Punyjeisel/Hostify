using Hostify.Domain.Entities;
using Hostify.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Hostify.Aplication.Features.Reviews.Interfaces;
using Hostify.Domain.Enums;
using Hostify.Aplication.Features.Reservations.DTOs;

namespace Hostify.Infrastructure.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly HostifyDbContext _context;

        public ReviewRepository(HostifyDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Review review)
        {
            await _context.reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Review>> GetByPropertyIdAsync(Guid propertyId)
        { 
            return await _context.reviews
                .Where(r => r.PropertyId == propertyId)
                .ToListAsync();
        }

        public async Task<bool> HasCompletedReservation(Guid userId, Guid propertyId)
        {
            return await _context.reservations.AnyAsync(r =>
                r.PropertyId == propertyId && 
                r.GuestId == userId && 
                r.Status == ReservationStatus.Completed
            );
        }

        public async Task<bool> ExistsByReservationIdAsync(Guid reservationId)
        {
            return await _context.reviews
                .AnyAsync(r => r.ReservationId == reservationId);
        }

        public async Task<Review?> GetByIdAsync(Guid id)
        {
            return await _context.reviews
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task UpdateAsync(Review review)
        {
            _context.reviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task<Review?> GetByReservationIdAsync(Guid reservationId)
        {
            return await _context.reviews
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);
        }
    }
}
