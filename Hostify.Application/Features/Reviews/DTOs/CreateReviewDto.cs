using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hostify.Application.Features.Reviews.DTOs
{
    public class CreateReviewDto
    {
        public Guid ReservationId { get; set; }
        public Guid PropertyId { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
