using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Domain.Entities;

namespace Hostify.Application.Features.Notifications.Interfaces
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<List<Notification>> GetByUserIdAsync(Guid userId);
        Task<Notification?> GetByIdAsync(Guid notificationId);
        Task MarkAsReadAsync(Guid notificationId);
    }
}
