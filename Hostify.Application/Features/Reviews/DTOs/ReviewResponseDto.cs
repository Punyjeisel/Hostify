using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hostify.Application.Features.Reviews.DTOs
{
    public class ReviewResponseDto
    {
        public Guid Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public string PropertyTitle { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
