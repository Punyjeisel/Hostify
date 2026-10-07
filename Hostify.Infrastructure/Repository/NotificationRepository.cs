using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Aplication.Features.Notifications.Interfaces;
using Hostify.Domain.Entities;
using Hostify.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace Hostify.Infrastructure.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly HostifyDbContext _context;

        public NotificationRepository(HostifyDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<Notification> GetByIdAsync(Guid userId)
        {
            return await _context.Set<Notification>()
                .FirstOrDefaultAsync(n => n.UserId == userId);
        }

        public async Task<List<Notification>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Set<Notification>()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _context.notifications.FindAsync(notificationId);

            if (notification != null)
                throw new Exception("Notificacion no encontrada");

            notification.IsRead = true;

            await _context.SaveChangesAsync();
        }
    }
}
