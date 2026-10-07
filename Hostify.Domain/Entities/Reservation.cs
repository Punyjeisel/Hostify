using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Domain.Enums;

namespace Hostify.Domain.Entities
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public Property Property { get; set; } = null!;
        public Guid GuestId { get; set; }
        public User Guest { get; set; } = null!;
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public ReservationStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    }
}
