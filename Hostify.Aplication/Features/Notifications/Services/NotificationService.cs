using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Aplication.Features.Notifications.Interfaces;
using Hostify.Domain.Entities;

namespace Hostify.Aplication.Features.Notifications.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        // Crea una nueva notificación para un usuario específico
        public async Task CreateAsync(Guid userId, string message)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Message = message,
                IsRead = false,
            };
            await _notificationRepository.AddAsync(notification);
        }

        // Obtiene todas las notificaciones de un usuario específico
        public async Task<List<Notification>> GetMyNotificationsAsync(Guid userId)
        {
            return await _notificationRepository.GetByUserIdAsync(userId);
        }

        // Marca una notificación como leída para un usuario específico, verificando que el usuario tenga permiso para acceder a esa notificación
        public async Task MarkAsReadAsync(Guid notificationId, Guid userId)
        {
            var notification = await _notificationRepository.GetByIdAsync(userId); 

            if (notification == null)
                throw new Exception("Notificación no encontrada");

            if (notification.UserId != userId)
                throw new Exception("No tienes permiso para esta notificación");

            await _notificationRepository.MarkAsReadAsync(notificationId);
        }
    }
}
