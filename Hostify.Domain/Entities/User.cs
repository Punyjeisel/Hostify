using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Domain.Enums;

namespace Hostify.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string passwordHash { get; set; } = string.Empty;
        public bool IsEmailConfirmed { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public ICollection<Property> Properties { get; set; } = new List<Property>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public ICollection<UserRoleEntity> Roles {  get; set; } = new List<UserRoleEntity>();
    }
}
