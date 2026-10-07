using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Domain.Enums;

namespace Hostify.Domain.Entities
{
    public class UserRoleEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public UserRole Role { get; set; }
    }
}
