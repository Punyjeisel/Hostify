using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Aplication.Features.Reviews.Interfaces;
using Hostify.Aplication.Features.Reviews.DTOs;
using Hostify.Domain.Entities;
using Hostify.Aplication.Features.Properties.Interfaces;

namespace Hostify.Aplication.Features.Reviews.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        // Este método permite a un usuario crear una reseña para una propiedad, pero solo si ha completado una reserva en esa propiedad.
        public async Task CreateReviewAsync(Guid userId, CreateReviewDto dto)
        {
            // Verificar si el usuario ha completado una reserva para la propiedad antes de permitir la creación de la reseña.
            var canReview = await _reviewRepository.HasCompletedReservation(userId, dto.PropertyId);

            if (!canReview)
                throw new InvalidOperationException("Solo puedes reseñar propiedades en las que hayas completado una reserva.");

            var reservationReviewed = await _reviewRepository.GetByReservationIdAsync(dto.ReservationId);

            if (reservationReviewed != null)
                throw new InvalidOperationException("Ya existe una reseña para esta reserva.");

            // Crear la reseña y guardarla en el repositorio.
            var review = new Review
            {
                Id = Guid.NewGuid(),
                ReservationId = dto.ReservationId,
                PropertyId = dto.PropertyId,
                UserId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _reviewRepository.AddAsync(review);
        }

        // Este método permite a cualquier usuario obtener todas las reseñas de una propiedad específica, lo que es útil para futuros huéspedes que quieran conocer la experiencia de otros usuarios antes de reservar.
        public async Task<List<ReviewResponseDto>> GetReviewsByPropertyIdAsync(Guid propertyId)
        {
            var reviews = await _reviewRepository.GetByPropertyIdAsync(propertyId);

            return reviews.Select(r => new ReviewResponseDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                PropertyTitle = "Review",
                CreatedAt = r.CreatedAt
            }).ToList();
        }

        public async Task UpdateReviewAsync(Guid reviewId, Guid userId, UpdateReviewDto dto)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);

            if (review == null)
                throw new InvalidOperationException("Reseña no encontrada.");

            if (review.UserId != userId)
                throw new Exception("No tienes permiso para actualizar esta reseña.");

            review.Rating = dto.Rating;
            review.Comment = dto.Comment;


            await _reviewRepository.UpdateAsync(review);

        }
    }
}
