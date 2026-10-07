using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Domain.Enums;

namespace Hostify.Application.Features.Reservations.DTOs
{
    public class ReservationResponseDto
    {
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string PropertyTitle { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public ReservationStatus Status { get; set; }
    }
}
