using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Domain.Entities;

namespace Hostify.Application.Features.Notifications.Interfaces
{
    public interface INotificationService
    {
        Task CreateAsync(Guid userId, string message);
        Task<List<Notification>> GetMyNotificationsAsync(Guid userId);
        Task MarkAsReadAsync(Guid notificationId, Guid userId);
    }
}
