using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hostify.Domain.Entities
{
    public class Property
    {
        public Guid Id { get; set; }
        public Guid HostId { get; set; }
        public User Host { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<BlockedDate> BlockedDates { get; set; } = new List<BlockedDate>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
